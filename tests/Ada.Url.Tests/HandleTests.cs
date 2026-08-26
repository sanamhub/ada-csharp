using System.Text;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// The storable tier. <see cref="AdaUrl"/> cannot leave its stack frame, so this is what a URL
/// held in a field or used across an await has to be.
/// </summary>
public class HandleTests
{
    [Fact]
    public void Parse_ReadsComponents()
    {
        using AdaUrlHandle url = AdaUrlHandle.Parse("https://user:pass@example.com:8443/a/../b?q=1#f"u8);

        Assert.Equal("example.com", Encoding.UTF8.GetString(url.GetHostname()));
        Assert.Equal("8443", Encoding.UTF8.GetString(url.GetPort()));
        Assert.Equal("/b", Encoding.UTF8.GetString(url.GetPathname()));
        Assert.Equal("?q=1", Encoding.UTF8.GetString(url.GetSearch()));
        Assert.Equal("#f", Encoding.UTF8.GetString(url.GetHash()));
        Assert.True(url.HasCredentials);
        Assert.True(url.HasPort);
        Assert.True(url.HasSearch);
    }

    [Fact]
    public void TryParse_RejectsBadInputWithoutLeaking()
    {
        // ada_parse returns a handle even when it could not parse, so the failure path has to
        // free it. Looping gives a leak something to show up as under the sanitizer.
        for (int i = 0; i < 1000; i++)
        {
            Assert.False(AdaUrlHandle.TryParse("not a url"u8, out AdaUrlHandle? url));
            Assert.Null(url);

            // Null on this path, so this disposes nothing. Present because a failed TryParse
            // returning a live handle is exactly the bug this test exists to catch, and the
            // call would matter the moment that regressed.
            url?.Dispose();
        }
    }

    [Fact]
    public void CanBeStoredInAField_WhichIsThePoint()
    {
        // This is the whole reason the type exists. The equivalent line with AdaUrl does not
        // compile, because a ref struct cannot reach a field.
        var holder = new Holder(AdaUrlHandle.Parse("https://example.com/a"u8));

        try
        {
            Assert.Equal("example.com", Encoding.UTF8.GetString(holder.Url.GetHostname()));
        }
        finally
        {
            holder.Url.Dispose();
        }
    }

    [Fact]
    public async Task SurvivesAnAwait()
    {
        using AdaUrlHandle url = AdaUrlHandle.Parse("https://example.com/before"u8);

        await Task.Yield();

        Assert.Equal("/before", Encoding.UTF8.GetString(url.GetPathname()));
    }

    [Fact]
    public void Setter_ChangesTheUrl()
    {
        using AdaUrlHandle url = AdaUrlHandle.Parse("https://example.org/file.txt"u8);

        Assert.True(url.TrySetHost("example.com"u8));
        Assert.Equal("https://example.com/file.txt", url.ToString());
    }

    [Fact]
    public void TryGetOrigin_FreesTheOwnedNativeString()
    {
        using AdaUrlHandle url = AdaUrlHandle.Parse("https://example.com:8443/x"u8);

        Span<byte> buffer = stackalloc byte[64];
        for (int i = 0; i < 1000; i++)
        {
            Assert.True(url.TryGetOrigin(buffer, out int written));
            Assert.Equal("https://example.com:8443", Encoding.UTF8.GetString(buffer[..written]));
        }
    }

    [Fact]
    public void Dispose_IsIdempotentAndMarksTheHandleClosed()
    {
        AdaUrlHandle url = AdaUrlHandle.Parse("https://example.com/"u8);
        Assert.False(url.IsClosed);

        url.Dispose();
        url.Dispose();

        Assert.True(url.IsClosed);
    }

    [Fact]
    public void ParseWithBase_ResolvesRelative()
    {
        Assert.True(AdaUrlHandle.TryParse("../b"u8, "https://example.org/one/two/three"u8, out AdaUrlHandle? url));
        Assert.NotNull(url);

        using (url)
        {
            Assert.Equal("https://example.org/one/b", url.ToString());
        }
    }

    [Fact]
    public void AgreesWithTheRefStructTier()
    {
        // The two tiers differ in lifetime, not in behaviour. If they ever disagree, one of them
        // is wrong.
        ReadOnlySpan<byte> input = "https://Bücher.example/a/../b?q=1#f"u8;

        using AdaUrlHandle viaHandle = AdaUrlHandle.Parse(input);
        Assert.True(AdaUrl.TryParse(input, out AdaUrl viaRefStruct));

        using (viaRefStruct)
        {
            Assert.Equal(
                Encoding.UTF8.GetString(viaRefStruct.Href),
                Encoding.UTF8.GetString(viaHandle.GetHref()));
        }
    }

    private sealed class Holder(AdaUrlHandle url)
    {
        public AdaUrlHandle Url { get; } = url;
    }
}
