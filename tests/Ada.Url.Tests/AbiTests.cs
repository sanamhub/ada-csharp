using System.Runtime.CompilerServices;
using System.Text;
using Ada.Url.Interop;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// Checks that the managed view of the C ABI matches the native one.
/// </summary>
/// <remarks>
/// <para>
/// This is the highest risk and lowest visibility failure mode in the whole design.
/// <c>ada_string</c> is 16 bytes on a 64 bit target, and the platforms disagree about how a
/// struct that size comes back from a function. Win64 returns it through a hidden pointer, SysV
/// x86-64 returns it in RAX and RDX, AArch64 in X0 and X1. .NET handles all three correctly for
/// a blittable struct, but a mismatch here is memory corruption rather than a bug report.
/// </para>
/// <para>
/// If any test in this class fails on a platform, that platform does not ship. There is no
/// workaround worth having.
/// </para>
/// </remarks>
[Trait("Category", "Abi")]
public class AbiTests
{
    [Fact]
    public void AdaString_HasTwoPointerSizedFields()
    {
        Assert.Equal(2 * nint.Size, Unsafe.SizeOf<AdaString>());
    }

    [Fact]
    public void AdaOwnedString_MatchesAdaString()
    {
        // They are the same shape in the header. If that ever stops being true, the free calls
        // in this wrapper start corrupting memory.
        Assert.Equal(Unsafe.SizeOf<AdaString>(), Unsafe.SizeOf<AdaOwnedString>());
    }

    [Fact]
    public void AdaStringPair_IsTwoStrings()
    {
        Assert.Equal(2 * Unsafe.SizeOf<AdaString>(), Unsafe.SizeOf<AdaStringPair>());
    }

    [Fact]
    public void AdaUrlComponents_IsEightUInt32Fields()
    {
        Assert.Equal(32, Unsafe.SizeOf<AdaUrlComponentsNative>());
    }

    [Fact]
    public void AdaVersionComponents_IsThreeInt32Fields()
    {
        Assert.Equal(12, Unsafe.SizeOf<AdaVersionComponents>());
    }

    [Fact]
    public void InteropStructs_AreBlittable()
    {
        // A managed reference anywhere in one of these forces a marshalling stub, which is both
        // a hidden allocation and a silent break of the zero allocation claim.
        AssertBlittable<AdaString>();
        AssertBlittable<AdaOwnedString>();
        AssertBlittable<AdaStringPair>();
        AssertBlittable<AdaUrlComponentsNative>();
        AssertBlittable<AdaVersionComponents>();
    }

    private static void AssertBlittable<T>()
        => Assert.False(
            RuntimeHelpers.IsReferenceOrContainsReferences<T>(),
            $"{typeof(T).Name} is not blittable.");

    [Fact]
    public void OmittedSentinel_MatchesTheHeader()
    {
        // ada_url_omitted is 0xffffffff, taken from ada_c.h at the pinned tag. Every uint field
        // of the components struct can carry it, not just port.
        Assert.Equal(0xFFFFFFFFu, AdaNative.Omitted);
        Assert.Equal(AdaNative.Omitted, AdaUrlComponents.Omitted);
        Assert.Equal(uint.MaxValue, AdaUrlComponents.Omitted);
    }

    [Fact]
    [Trait("Category", "Abi")]
    public void StructReturnByValue_ReconstructsExactBytes()
    {
        // The direct test for sret versus register pair mishandling. If the calling convention
        // is wrong, either the pointer or the length comes back as garbage.
        const string Input = "https://user:pass@example.com:8443/a/b?q=1#frag";
        byte[] utf8 = Encoding.UTF8.GetBytes(Input);

        Assert.True(AdaUrl.TryParse(utf8, out AdaUrl url));
        using (url)
        {
            Assert.Equal(Input, Encoding.UTF8.GetString(url.Href));
            Assert.Equal("example.com", Encoding.UTF8.GetString(url.Hostname));
            Assert.Equal("8443", Encoding.UTF8.GetString(url.Port));
            Assert.Equal("/a/b", Encoding.UTF8.GetString(url.Pathname));
            Assert.Equal("?q=1", Encoding.UTF8.GetString(url.Search));
            Assert.Equal("#frag", Encoding.UTF8.GetString(url.Hash));
            Assert.Equal("https:", Encoding.UTF8.GetString(url.Protocol));
            Assert.Equal("user", Encoding.UTF8.GetString(url.Username));
            Assert.Equal("pass", Encoding.UTF8.GetString(url.Password));
        }
    }

    [Fact]
    public void VersionComponents_AgreeWithTheVersionString()
    {
        // A cheap end to end proof that the ABI is aligned at all. If the struct return is
        // misread, these two disagree.
        string version = AdaLibrary.NativeVersion;
        (int major, int minor, int revision) = AdaLibrary.GetNativeVersionComponents();

        Assert.Equal($"{major}.{minor}.{revision}", version);
        Assert.Equal(AdaLibrary.PinnedVersion, version);
    }

    [Fact]
    public void CBool_IsExactlyZeroOrOne()
    {
        // C _Bool is one byte. If the upper bytes of the return register leak through, a
        // predicate can read as true when the native side said false.
        byte[] utf8 = Encoding.UTF8.GetBytes("https://example.com/");

        Assert.True(AdaUrl.TryParse(utf8, out AdaUrl url));
        using (url)
        {
            Assert.False(url.HasPort);
            Assert.False(url.HasCredentials);
            Assert.False(url.HasSearch);
            Assert.False(url.HasHash);
            Assert.True(url.HasHostname);
        }
    }

    [Fact]
    public void MaxInputLength_RoundTripsThroughGlobalState()
    {
        // size_t and uint32 width check on the global setter, and a reminder that this is
        // process wide state.
        uint original = AdaLibrary.MaxInputLength;
        try
        {
            AdaLibrary.MaxInputLength = 4096;
            Assert.Equal(4096u, AdaLibrary.MaxInputLength);
        }
        finally
        {
            AdaLibrary.MaxInputLength = original;
        }
    }
}
