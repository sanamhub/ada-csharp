using System.Text;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// Long running checks, excluded from pull request CI by trait and run nightly.
/// </summary>
/// <remarks>
/// These exist to give a leak or a use after free enough repetitions to become visible. On their
/// own they only prove the library survives; paired with the sanitizer job they are what turns
/// "no crash" into "no leak".
/// </remarks>
[Trait("Category", "Stress")]
public class SoakTests
{
    private static readonly string[] Corpus =
    [
        "https://example.com/",
        "https://user:pass@example.com:8443/a/../b?q=1#frag",
        "https://Bücher.example/path",
        "http://192.168.0.1:8080/x",
        "https://[2001:db8::1]/y",
        "file:///C:/temp/file.txt",
        "ws://example.com/socket",
        "not a url at all",
        "",
        "https://example.com/" + new string('a', 2048),
    ];

    [Fact]
    public void ParseAndRead_OverManyIterations()
    {
        byte[][] inputs = Array.ConvertAll(Corpus, Encoding.UTF8.GetBytes);
        long total = 0;

        for (int i = 0; i < 200_000; i++)
        {
            byte[] input = inputs[i % inputs.Length];
            if (!AdaUrl.TryParse(input, out AdaUrl url))
            {
                // Invalid inputs are in the corpus on purpose. The failure path still allocates
                // a native handle that has to be freed, which is the leak worth exercising.
                url.Dispose();
                continue;
            }

            using (url)
            {
                total += url.Href.Length + url.Hostname.Length + url.Pathname.Length;
            }
        }

        Assert.True(total > 0);
    }

    [Fact]
    public void MutateAndReserialise_OverManyIterations()
    {
        ReadOnlySpan<byte> host = "example.org"u8;

        for (int i = 0; i < 100_000; i++)
        {
            if (!AdaUrl.TryParse("https://example.com/a/b?q=1#f"u8, out AdaUrl url))
            {
                continue;
            }

            using (url)
            {
                url.TrySetHost(host);
                url.SetSearch("?x=2"u8);
                url.ClearHash();
                Assert.False(url.Href.IsEmpty);
            }
        }
    }

    [Fact]
    public void OwnedNativeStrings_OverManyIterations()
    {
        // Origin and IDNA both return memory the caller owns. A missed free here is the leak
        // most likely to reach a release, because nothing in normal use makes it visible.
        Span<byte> buffer = stackalloc byte[128];

        for (int i = 0; i < 100_000; i++)
        {
            Assert.True(AdaUrl.TryGetOrigin("https://example.com:8443/x"u8, buffer, out _));
            Assert.True(AdaIdna.TryToAscii("Bücher.example"u8, buffer, out _));
        }
    }

    [Fact]
    public void SearchParamsIterators_OverManyIterations()
    {
        // Every enumeration allocates a native iterator that the enumerator has to free.
        for (int i = 0; i < 100_000; i++)
        {
            using var parameters = AdaSearchParams.Parse("a=1&b=2&c=3"u8);
            int count = 0;
            foreach (AdaSearchParams.Entry entry in parameters)
            {
                count += entry.Value.Length;
            }

            Assert.Equal(3, count);
        }
    }

    [Fact]
    public void Handles_OverManyIterations()
    {
        for (int i = 0; i < 100_000; i++)
        {
            using AdaUrlHandle url = AdaUrlHandle.Parse("https://example.com/a?b=1"u8);
            Assert.False(url.GetHref().IsEmpty);
        }
    }
}
