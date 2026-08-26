using System.Text;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// Behaviour checks for the public surface. The full WHATWG corpus arrives in P3, so these are
/// the cases worth catching before then, chosen because each one has burned somebody.
/// </summary>
public class ParsingTests
{
    [Fact]
    public void Parse_RemovesDotSegments()
    {
        // The example from the ada-python README.
        Assert.True(AdaUrl.TryParse("https://example.org/path/../file.txt"u8, out AdaUrl url));
        using (url)
        {
            Assert.Equal("https://example.org/file.txt", Encoding.UTF8.GetString(url.Href));
        }
    }

    [Fact]
    public void Parse_KeepsANonDefaultPort()
    {
        // 443 is the https default, 80 is not, so :80 stays. Easy to assume backwards.
        Assert.True(AdaUrl.TryParse("https://example.org:80/api"u8, out AdaUrl url));
        using (url)
        {
            Assert.Equal("80", Encoding.UTF8.GetString(url.Port));
            Assert.True(url.HasPort);
        }
    }

    [Fact]
    public void Parse_DropsTheDefaultPort()
    {
        Assert.True(AdaUrl.TryParse("https://example.org:443/api"u8, out AdaUrl url));
        using (url)
        {
            Assert.True(url.Port.IsEmpty);
            Assert.False(url.HasPort);
        }
    }

    [Fact]
    public void Parse_ResolvesAgainstABase()
    {
        Assert.True(AdaUrl.TryParse("../b"u8, "https://example.org/one/two/three"u8, out AdaUrl url));
        using (url)
        {
            Assert.Equal("https://example.org/b", Encoding.UTF8.GetString(url.Href));
        }
    }

    [Fact]
    public void Parse_NormalisesAnInternationalisedDomain()
    {
        Assert.True(AdaUrl.TryParse("https://Bücher.example/"u8, out AdaUrl url));
        using (url)
        {
            Assert.Equal("xn--bcher-kva.example", Encoding.UTF8.GetString(url.Hostname));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(":")]
    [InlineData("http:")]
    [InlineData("//")]
    [InlineData("not a url at all")]
    public void TryParse_RejectsBadInputWithoutLeaking(string input)
    {
        // ada_parse returns a handle even when it could not parse, so the wrapper has to free it.
        // Running this many times is how a leak here would show up under the sanitizer.
        byte[] utf8 = Encoding.UTF8.GetBytes(input);
        for (int i = 0; i < 1000; i++)
        {
            Assert.False(AdaUrl.TryParse(utf8, out AdaUrl url));
            url.Dispose();
        }
    }

    [Fact]
    public void Parse_AcceptsUtf16AndAgreesWithUtf8()
    {
        const string Input = "https://example.com/café?q=✓";

        Assert.True(AdaUrl.TryParse(Input.AsSpan(), out AdaUrl fromUtf16));
        Assert.True(AdaUrl.TryParse(Encoding.UTF8.GetBytes(Input), out AdaUrl fromUtf8));

        using (fromUtf16)
        using (fromUtf8)
        {
            Assert.Equal(Encoding.UTF8.GetString(fromUtf8.Href), Encoding.UTF8.GetString(fromUtf16.Href));
        }
    }

    [Fact]
    public void TryParse_RejectsALoneSurrogate()
    {
        // Must come back as false rather than throwing. An exception escaping here would make
        // exceptions control flow on the parse path.
        string input = "https://example.com/" + '\ud800';
        Assert.False(AdaUrl.TryParse(input.AsSpan(), out AdaUrl url));
        url.Dispose();
    }

    [Fact]
    public void TryParse_HandlesInputLongerThanTheStackThreshold()
    {
        // Forces the array pool path instead of stackalloc.
        string input = "https://example.com/" + new string('a', 4096);
        Assert.True(AdaUrl.TryParse(input.AsSpan(), out AdaUrl url));
        using (url)
        {
            Assert.Equal(4116, url.Href.Length);
        }
    }

    [Fact]
    public void Setter_ChangesTheSerialisedUrl()
    {
        Assert.True(AdaUrl.TryParse("https://example.org/file.txt"u8, out AdaUrl url));
        using (url)
        {
            Assert.True(url.TrySetHost("example.com"u8));

            // The span read before the setter is now invalid, which is why we re-read it.
            Assert.Equal("https://example.com/file.txt", Encoding.UTF8.GetString(url.Href));
        }
    }

    [Fact]
    public void Components_UseTheOmittedSentinelForAbsentParts()
    {
        Assert.True(AdaUrl.TryParse("https://example.com/path"u8, out AdaUrl url));
        using (url)
        {
            AdaUrlComponents c = url.Components;

            Assert.False(AdaUrlComponents.IsPresent(c.Port));
            Assert.False(AdaUrlComponents.IsPresent(c.SearchStart));
            Assert.False(AdaUrlComponents.IsPresent(c.HashStart));
            Assert.True(AdaUrlComponents.IsPresent(c.HostStart));
            Assert.True(AdaUrlComponents.IsPresent(c.PathnameStart));
        }
    }

    [Fact]
    public void CanParse_AnswersWithoutBuildingAUrl()
    {
        Assert.True(AdaUrl.CanParse("https://example.com/"u8));
        Assert.False(AdaUrl.CanParse("not a url"u8));
        Assert.True(AdaUrl.CanParse("../b"u8, "https://example.org/one/two"u8));
    }

    [Fact]
    public void TryNormalize_WritesIntoACallerBuffer()
    {
        Span<byte> buffer = stackalloc byte[128];
        Assert.True(AdaUrl.TryNormalize("https://example.org/path/../file.txt"u8, buffer, out int written));
        Assert.Equal("https://example.org/file.txt", Encoding.UTF8.GetString(buffer[..written]));
    }

    [Fact]
    public void TryNormalize_ReportsABufferThatIsTooSmall()
    {
        Span<byte> buffer = stackalloc byte[4];
        Assert.False(AdaUrl.TryNormalize("https://example.org/file.txt"u8, buffer, out int written));
        Assert.Equal(0, written);
    }

    [Fact]
    public void TryGetHostname_ExtractsTheHostForAnAllowListCheck()
    {
        Span<byte> buffer = stackalloc byte[64];
        Assert.True(AdaUrl.TryGetHostname("https://user:pass@example.com:8443/x"u8, buffer, out int written));
        Assert.Equal("example.com", Encoding.UTF8.GetString(buffer[..written]));
    }

    [Fact]
    public void TryGetOrigin_FreesTheOwnedNativeString()
    {
        // ada_get_origin returns owned memory. Looping proves the wrapper releases it.
        Span<byte> buffer = stackalloc byte[64];
        for (int i = 0; i < 1000; i++)
        {
            Assert.True(AdaUrl.TryGetOrigin("https://example.com:8443/x"u8, buffer, out int written));
            Assert.Equal("https://example.com:8443", Encoding.UTF8.GetString(buffer[..written]));
        }
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        Assert.True(AdaUrl.TryParse("https://example.com/"u8, out AdaUrl url));
        url.Dispose();
        url.Dispose();
        Assert.False(url.IsValid);
    }

    [Fact]
    public void HostType_AndSchemeType_AreReported()
    {
        Assert.True(AdaUrl.TryParse("https://192.168.0.1/"u8, out AdaUrl ipv4));
        using (ipv4)
        {
            Assert.Equal(AdaHostType.IPv4, ipv4.HostType);
            Assert.Equal(AdaSchemeType.Https, ipv4.SchemeType);
        }

        Assert.True(AdaUrl.TryParse("https://example.com/"u8, out AdaUrl domain));
        using (domain)
        {
            Assert.Equal(AdaHostType.Domain, domain.HostType);
        }
    }
}

/// <summary>
/// The zero allocation claim, asserted rather than measured.
/// </summary>
/// <remarks>
/// BenchmarkDotNet is too noisy on shared hardware to gate anything. This is deterministic, so
/// it is the check that actually keeps the claim honest.
/// </remarks>
public class AllocationTests
{
    [Fact]
    public void ParseAndReadAll_AllocatesNothing()
    {
        ReadOnlySpan<byte> input = "https://user:pass@example.com:8443/a/b?q=1#frag"u8;

        // Warm up first. The JIT and the array pool both allocate on first touch, and charging
        // that to the measurement would make this fail for the wrong reason.
        for (int i = 0; i < 10_000; i++)
        {
            Consume(input);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100_000; i++)
        {
            Consume(input);
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    [Fact]
    public void CanParse_AllocatesNothing()
    {
        ReadOnlySpan<byte> input = "https://example.com/path?q=1"u8;

        for (int i = 0; i < 10_000; i++)
        {
            AdaUrl.CanParse(input);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100_000; i++)
        {
            AdaUrl.CanParse(input);
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    [Fact]
    public void Utf16Parse_AllocatesNothingBelowTheStackThreshold()
    {
        const string Input = "https://example.com/path?q=1";

        for (int i = 0; i < 10_000; i++)
        {
            ConsumeUtf16(Input);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 100_000; i++)
        {
            ConsumeUtf16(Input);
        }

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    private static int Consume(ReadOnlySpan<byte> input)
    {
        if (!AdaUrl.TryParse(input, out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            // Touch every borrowed getter so the whole read path is covered.
            return url.Href.Length + url.Protocol.Length + url.Host.Length + url.Hostname.Length
                 + url.Port.Length + url.Pathname.Length + url.Search.Length + url.Hash.Length
                 + url.Username.Length + url.Password.Length;
        }
    }

    private static int ConsumeUtf16(ReadOnlySpan<char> input)
    {
        if (!AdaUrl.TryParse(input, out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return url.Href.Length;
        }
    }
}
