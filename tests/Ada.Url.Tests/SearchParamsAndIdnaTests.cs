using System.Text;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// Covers the two pieces of parity with the official Python and Go bindings: URLSearchParams and
/// the IDNA helpers. The documented examples from those bindings are asserted directly, because
/// the failure worth catching is a wrapper that is self consistent but disagrees with every other
/// Ada binding.
/// </summary>
public class SearchParamsTests
{
    [Fact]
    public void Enumerates_InOrder()
    {
        // Straight from the ada-python README.
        using var parameters = AdaSearchParams.Parse("key1=value1&key2=value2"u8);

        var seen = new List<(string Key, string Value)>();
        foreach (AdaSearchParams.Entry entry in parameters)
        {
            seen.Add((Encoding.UTF8.GetString(entry.Key), Encoding.UTF8.GetString(entry.Value)));
        }

        Assert.Equal([("key1", "value1"), ("key2", "value2")], seen);
    }

    [Fact]
    public void Count_CountsDuplicateKeysSeparately()
    {
        using var parameters = AdaSearchParams.Parse("a=1&a=2&b=3"u8);
        Assert.Equal(3, parameters.Count);
    }

    [Fact]
    public void Get_ReturnsTheFirstValue()
    {
        using var parameters = AdaSearchParams.Parse("a=1&a=2"u8);
        Assert.Equal("1", Encoding.UTF8.GetString(parameters.Get("a"u8)));
    }

    [Fact]
    public void Get_ReturnsEmptyForAnAbsentKey()
    {
        using var parameters = AdaSearchParams.Parse("a=1"u8);
        Assert.True(parameters.Get("missing"u8).IsEmpty);
        Assert.False(parameters.Has("missing"u8));
    }

    [Fact]
    public void Append_KeepsExistingPairs_SetReplacesThem()
    {
        using var parameters = AdaSearchParams.Parse("a=1"u8);

        parameters.Append("a"u8, "2"u8);
        Assert.Equal(2, parameters.Count);

        parameters.Set("a"u8, "3"u8);
        Assert.Equal(1, parameters.Count);
        Assert.Equal("3", Encoding.UTF8.GetString(parameters.Get("a"u8)));
    }

    [Fact]
    public void Remove_DropsEveryPairWithThatKey()
    {
        using var parameters = AdaSearchParams.Parse("a=1&a=2&b=3"u8);
        parameters.Remove("a"u8);

        Assert.Equal(1, parameters.Count);
        Assert.False(parameters.Has("a"u8));
        Assert.True(parameters.Has("b"u8));
    }

    [Fact]
    public void Sort_OrdersByKeyAndKeepsEqualKeysStable()
    {
        using var parameters = AdaSearchParams.Parse("c=3&a=1&a=2&b=4"u8);
        parameters.Sort();

        Assert.Equal("a=1&a=2&b=4&c=3", parameters.ToString());
    }

    [Fact]
    public void TryToString_WritesIntoACallerBuffer()
    {
        using var parameters = AdaSearchParams.Parse("a=1&b=2"u8);

        Span<byte> buffer = stackalloc byte[64];
        Assert.True(parameters.TryToString(buffer, out int written));
        Assert.Equal("a=1&b=2", Encoding.UTF8.GetString(buffer[..written]));
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var parameters = AdaSearchParams.Parse("a=1"u8);
        parameters.Dispose();
        parameters.Dispose();
    }

    [Fact]
    public void RepeatedEnumeration_DoesNotLeakIterators()
    {
        // Each enumeration allocates a native iterator that the enumerator has to free. A leak
        // here shows up under the sanitizer, and this loop is what gives it something to find.
        using var parameters = AdaSearchParams.Parse("a=1&b=2&c=3"u8);

        for (int i = 0; i < 2000; i++)
        {
            int count = 0;
            foreach (AdaSearchParams.Entry entry in parameters)
            {
                count += entry.Key.Length;
            }

            Assert.Equal(3, count);
        }
    }
}

/// <summary>
/// IDNA conversion, which matters for security rather than display. See the remarks on
/// <see cref="AdaIdna"/>.
/// </summary>
public class IdnaTests
{
    [Fact]
    public void ToAscii_MatchesThePythonBindingExample()
    {
        Assert.Equal("xn--bcher-kva.example", AdaIdna.ToAscii("Bücher.example"));
    }

    [Fact]
    public void ToUnicode_MatchesThePythonBindingExample()
    {
        Assert.Equal("bücher.example", AdaIdna.ToUnicode("xn--bcher-kva.example"));
    }

    [Fact]
    public void ToAscii_LowercasesAndRoundTrips()
    {
        string ascii = AdaIdna.ToAscii("Bücher.example");
        Assert.Equal("bücher.example", AdaIdna.ToUnicode(ascii));
    }

    [Fact]
    public void TryToAscii_WritesIntoACallerBuffer()
    {
        Span<byte> buffer = stackalloc byte[64];
        Assert.True(AdaIdna.TryToAscii("Bücher.example"u8, buffer, out int written));
        Assert.Equal("xn--bcher-kva.example", Encoding.UTF8.GetString(buffer[..written]));
    }

    [Fact]
    public void TryToAscii_ReportsABufferThatIsTooSmall()
    {
        Span<byte> buffer = stackalloc byte[4];
        Assert.False(AdaIdna.TryToAscii("Bücher.example"u8, buffer, out int written));
        Assert.Equal(0, written);
    }

    [Fact]
    public void ConfusableDomains_MapToDifferentAscii()
    {
        // The security point, made concrete. These two look near identical to a person: the
        // second uses Cyrillic а and е. They must not compare equal after IDNA, which is why an
        // allow list has to match on the ASCII form.
        string latin = AdaIdna.ToAscii("example.com");
        string cyrillic = AdaIdna.ToAscii("exаmplе.com");

        Assert.NotEqual(latin, cyrillic);
        Assert.Equal("example.com", latin);
    }

    [Fact]
    public void RepeatedConversion_DoesNotLeakOwnedStrings()
    {
        // Both IDNA functions return memory the caller owns. Looping proves the wrapper frees it.
        Span<byte> buffer = stackalloc byte[64];
        for (int i = 0; i < 2000; i++)
        {
            Assert.True(AdaIdna.TryToAscii("Bücher.example"u8, buffer, out int written));
            Assert.Equal(21, written);
        }
    }
}
