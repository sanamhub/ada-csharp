using System.Text;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// Covers the public members that the behaviour focused suites happened not to touch.
/// </summary>
/// <remarks>
/// Found by listing the public API baseline and checking each member against the test sources.
/// An untested public member is a member nobody has confirmed works, and shipping one in a
/// package is how a beta acquires its first bug report.
/// </remarks>
public class SurfaceCoverageTests
{
    [Fact]
    public void Predicates_DistinguishEmptyFromAbsent()
    {
        // Four predicates that look interchangeable and are not. A present but empty username is
        // a different state from no username, and the standard treats them differently.
        Assert.True(AdaUrl.TryParse("https://user:pass@example.com/"u8, out AdaUrl both));
        using (both)
        {
            Assert.True(both.HasCredentials);
            Assert.True(both.HasPassword);
            Assert.True(both.HasNonEmptyUsername);
            Assert.True(both.HasNonEmptyPassword);
        }

        Assert.True(AdaUrl.TryParse("https://user@example.com/"u8, out AdaUrl userOnly));
        using (userOnly)
        {
            Assert.True(userOnly.HasNonEmptyUsername);
            Assert.False(userOnly.HasPassword);
            Assert.False(userOnly.HasNonEmptyPassword);
        }

        Assert.True(AdaUrl.TryParse("https://example.com/"u8, out AdaUrl neither));
        using (neither)
        {
            Assert.False(neither.HasCredentials);
            Assert.False(neither.HasNonEmptyUsername);
            Assert.False(neither.HasPassword);
        }
    }

    [Fact]
    public void HasEmptyHostname_IsNotTheSameAsHavingNoHostname()
    {
        // A non special scheme can carry an empty host. Conflating that with "no host" is how an
        // allow list ends up letting something through.
        Assert.True(AdaUrl.TryParse("file:///tmp/x"u8, out AdaUrl empty));
        using (empty)
        {
            Assert.True(empty.HasEmptyHostname);
            Assert.True(empty.Hostname.IsEmpty);
        }

        Assert.True(AdaUrl.TryParse("https://example.com/"u8, out AdaUrl present));
        using (present)
        {
            Assert.False(present.HasEmptyHostname);
            Assert.True(present.HasHostname);
        }
    }

    [Fact]
    public void ClearPort_AndClearSearch_RemoveTheirComponents()
    {
        Assert.True(AdaUrl.TryParse("https://example.com:8443/a?q=1#f"u8, out AdaUrl url));
        using (url)
        {
            Assert.True(url.HasPort);
            Assert.True(url.HasSearch);

            url.ClearPort();
            url.ClearSearch();

            Assert.False(url.HasPort);
            Assert.False(url.HasSearch);
            Assert.Equal("https://example.com/a#f", Encoding.UTF8.GetString(url.Href));
        }
    }

    [Fact]
    public void ClearHash_RemovesTheFragment()
    {
        Assert.True(AdaUrl.TryParse("https://example.com/a#frag"u8, out AdaUrl url));
        using (url)
        {
            Assert.True(url.HasHash);
            url.ClearHash();
            Assert.False(url.HasHash);
            Assert.Equal("https://example.com/a", Encoding.UTF8.GetString(url.Href));
        }
    }

    [Theory]
    [InlineData("https://example.com/", AdaSchemeType.Https)]
    [InlineData("http://example.com/", AdaSchemeType.Http)]
    [InlineData("ws://example.com/", AdaSchemeType.Ws)]
    [InlineData("wss://example.com/", AdaSchemeType.Wss)]
    [InlineData("ftp://example.com/", AdaSchemeType.Ftp)]
    [InlineData("file:///tmp/x", AdaSchemeType.File)]
    [InlineData("custom-scheme://opaque/x", AdaSchemeType.NotSpecial)]
    public void SchemeType_MatchesTheScheme(string input, AdaSchemeType expected)
    {
        // These values come straight off the C ABI as a uint8. If the enum ever drifts from
        // upstream's ordering, every one of these breaks at once, which is the point.
        Assert.True(AdaUrl.TryParse(Encoding.UTF8.GetBytes(input), out AdaUrl url));
        using (url)
        {
            Assert.Equal(expected, url.SchemeType);
        }
    }

    [Theory]
    [InlineData("https://example.com/", AdaHostType.Domain)]
    [InlineData("https://192.168.0.1/", AdaHostType.IPv4)]
    [InlineData("https://[2001:db8::1]/", AdaHostType.IPv6)]
    public void HostType_MatchesTheHost(string input, AdaHostType expected)
    {
        Assert.True(AdaUrl.TryParse(Encoding.UTF8.GetBytes(input), out AdaUrl url));
        using (url)
        {
            Assert.Equal(expected, url.HostType);
        }
    }

    [Fact]
    public void Components_HostStartPointsAtTheAtSignWhenCredentialsArePresent()
    {
        // This is the trap the component offsets carry, and the reason the slicing fast path was
        // never built. With credentials in the URL, host_start indexes the '@' rather than the
        // first character of the host, so slicing [HostStart..HostEnd] yields "@example.com".
        // Upstream's own diagram in url_components.h shows the marker under the '@'.
        const string Input = "https://user:pw@example.com:8443/a/b?q=1#f";
        Assert.True(AdaUrl.TryParse(Encoding.UTF8.GetBytes(Input), out AdaUrl url));

        using (url)
        {
            AdaUrlComponents c = url.Components;
            ReadOnlySpan<byte> href = url.Href;

            Assert.True(AdaUrlComponents.IsPresent(c.ProtocolEnd));
            Assert.True(AdaUrlComponents.IsPresent(c.UsernameEnd));
            Assert.True(AdaUrlComponents.IsPresent(c.HostStart));
            Assert.True(AdaUrlComponents.IsPresent(c.HostEnd));

            Assert.Equal("https:", Encoding.UTF8.GetString(href[..(int)c.ProtocolEnd]));

            // Documented as it actually behaves, not as it reads.
            Assert.Equal("@example.com", Encoding.UTF8.GetString(href[(int)c.HostStart..(int)c.HostEnd]));

            // Which is why a caller has to skip it, and why Hostname is the safe way to ask.
            Assert.Equal("example.com", Encoding.UTF8.GetString(href[((int)c.HostStart + 1)..(int)c.HostEnd]));
            Assert.Equal("example.com", Encoding.UTF8.GetString(url.Hostname));

            Assert.Equal(8443u, c.Port);
        }
    }

    [Fact]
    public void Components_HostStartPointsAtTheHostWhenThereAreNoCredentials()
    {
        // Without credentials there is no '@', so the same slice is correct. Two different
        // behaviours from one field is exactly what makes it worth pinning.
        Assert.True(AdaUrl.TryParse("https://example.com:8443/a"u8, out AdaUrl url));

        using (url)
        {
            AdaUrlComponents c = url.Components;
            ReadOnlySpan<byte> href = url.Href;

            Assert.Equal("example.com", Encoding.UTF8.GetString(href[(int)c.HostStart..(int)c.HostEnd]));
        }
    }

    [Fact]
    public void Components_AreOffsetsSoDifferentPathsCanCompareEqual()
    {
        // Not a bug, and worth pinning because it surprises. The struct holds offsets and has no
        // field for where the path ends, so https://example.com/a and
        // https://example.com/completely/different produce byte identical components: same
        // protocolEnd, same hostEnd, same pathnameStart, and nothing that records the rest.
        //
        // Anyone treating component equality as URL equality gets a wrong answer here. Compare
        // Href for that.
        Assert.True(AdaUrl.TryParse("https://example.com/a"u8, out AdaUrl shortPath));
        Assert.True(AdaUrl.TryParse("https://example.com/completely/different"u8, out AdaUrl longPath));

        using (shortPath)
        using (longPath)
        {
            Assert.Equal(shortPath.Components, longPath.Components);
            Assert.NotEqual(
                Encoding.UTF8.GetString(shortPath.Href),
                Encoding.UTF8.GetString(longPath.Href));
        }
    }

    [Fact]
    public void Components_CompareByValue()
    {
        Assert.True(AdaUrl.TryParse("https://example.com/a"u8, out AdaUrl first));
        Assert.True(AdaUrl.TryParse("https://example.com/a"u8, out AdaUrl second));

        // Differs in host length, port, query and fragment, so the offsets genuinely move.
        Assert.True(AdaUrl.TryParse("https://a-much-longer-host.example:8443/a?q=1#f"u8, out AdaUrl third));

        using (first)
        using (second)
        using (third)
        {
            AdaUrlComponents a = first.Components;
            AdaUrlComponents b = second.Components;
            AdaUrlComponents c = third.Components;

            // Report the fields on failure. "Expected True, Actual False" on a struct comparison
            // says nothing about which field differed.
            string describe(AdaUrlComponents x) =>
                $"protocolEnd={x.ProtocolEnd} usernameEnd={x.UsernameEnd} hostStart={x.HostStart} " +
                $"hostEnd={x.HostEnd} port={x.Port} pathnameStart={x.PathnameStart} " +
                $"searchStart={x.SearchStart} hashStart={x.HashStart}";

            Assert.True(a == b, $"identical URLs gave different components. a: {describe(a)} | b: {describe(b)}");
            Assert.False(a != b);
            Assert.True(a.Equals(b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());

            Assert.True(a != c, $"different URLs gave equal components. a: {describe(a)} | c: {describe(c)}");
        }
    }

    [Fact]
    public void Idna_TryToUnicode_ReversesTryToAscii()
    {
        Span<byte> ascii = stackalloc byte[64];
        Assert.True(AdaIdna.TryToAscii("Bücher.example"u8, ascii, out int asciiLength));
        Assert.Equal("xn--bcher-kva.example", Encoding.UTF8.GetString(ascii[..asciiLength]));

        Span<byte> unicode = stackalloc byte[64];
        Assert.True(AdaIdna.TryToUnicode(ascii[..asciiLength], unicode, out int unicodeLength));
        Assert.Equal("bücher.example", Encoding.UTF8.GetString(unicode[..unicodeLength]));
    }

    [Fact]
    public void Idna_TryToUnicode_ReportsABufferThatIsTooSmall()
    {
        Span<byte> tiny = stackalloc byte[2];
        Assert.False(AdaIdna.TryToUnicode("xn--bcher-kva.example"u8, tiny, out int written));
        Assert.Equal(0, written);
    }

    [Fact]
    public void Handle_ExposesHostAndProtocol()
    {
        using AdaUrlHandle url = AdaUrlHandle.Parse("https://example.com:8443/a"u8);

        Assert.Equal("https:", Encoding.UTF8.GetString(url.GetProtocol()));
        Assert.Equal("example.com:8443", Encoding.UTF8.GetString(url.GetHost()));
        Assert.Equal("example.com", Encoding.UTF8.GetString(url.GetHostname()));
    }

    [Fact]
    public void Handle_IsInvalidReflectsDisposal()
    {
        AdaUrlHandle url = AdaUrlHandle.Parse("https://example.com/"u8);
        Assert.False(url.IsInvalid);

        url.Dispose();

        // SafeHandle reports closed rather than invalid after disposal. Both are worth pinning,
        // because a caller checking the wrong one gets a confusing answer.
        Assert.True(url.IsClosed);
    }

    [Fact]
    public void SearchParams_EnumeratorIsUsableDirectly()
    {
        // foreach exercises this, but the enumerator is public, so a caller can drive it by hand.
        using var parameters = AdaSearchParams.Parse("a=1&b=2"u8);

        using AdaSearchParams.Enumerator enumerator = parameters.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("a", Encoding.UTF8.GetString(enumerator.Current.Key));
        Assert.Equal("1", Encoding.UTF8.GetString(enumerator.Current.Value));

        Assert.True(enumerator.MoveNext());
        Assert.Equal("b", Encoding.UTF8.GetString(enumerator.Current.Key));

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void Library_ReportsItsPinnedAndLoadedVersions()
    {
        Assert.Equal("4.0.0", AdaLibrary.PinnedVersion);
        Assert.Equal(AdaLibrary.PinnedVersion, AdaLibrary.NativeVersion);

        (int major, int minor, int revision) = AdaLibrary.GetNativeVersionComponents();
        Assert.Equal(4, major);
        Assert.Equal(0, minor);
        Assert.Equal(0, revision);
    }
}
