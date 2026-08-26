using System.Runtime.InteropServices;

namespace Ada.Url.Interop;

/// <summary>
/// A string owned by the native library and borrowed by us. Valid only until the owning URL is
/// mutated or freed.
/// </summary>
/// <remarks>
/// Identical in layout to <see cref="AdaOwnedString"/>. The C header distinguishes them in prose
/// only, which is why they are separate types here. Getting the two confused is either a leak or
/// a double free.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct AdaString
{
    public readonly byte* Data;
    public readonly nuint Length;

    public ReadOnlySpan<byte> AsSpan() => Data is null ? default : new ReadOnlySpan<byte>(Data, (int)Length);
}

/// <summary>
/// A string allocated by the native library and owned by the caller. Must be released with
/// <c>ada_free_owned_string</c> or it leaks.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct AdaOwnedString
{
    public readonly byte* Data;
    public readonly nuint Length;

    public ReadOnlySpan<byte> AsSpan() => Data is null ? default : new ReadOnlySpan<byte>(Data, (int)Length);
}

/// <summary>
/// A borrowed key and value pair, returned by the search params entries iterator.
/// </summary>
/// <remarks>
/// 32 bytes on a 64 bit target, so it is returned through a hidden pointer on every ABI we
/// support. More likely to be mishandled than <see cref="AdaString"/>, which is why the ABI suite
/// tests it separately.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct AdaStringPair
{
    public readonly AdaString Key;
    public readonly AdaString Value;
}

/// <summary>
/// Byte offsets into the serialised href, as returned by <c>ada_get_components</c>.
/// </summary>
/// <remarks>
/// Every field can hold <see cref="AdaNative.Omitted"/> to mean the component is absent, not just
/// <see cref="Port"/>. Casting an omitted field to <see cref="int"/> gives -1 and an out of range
/// slice, so check before using any of them.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct AdaUrlComponentsNative
{
    public readonly uint ProtocolEnd;
    public readonly uint UsernameEnd;
    public readonly uint HostStart;
    public readonly uint HostEnd;
    public readonly uint Port;
    public readonly uint PathnameStart;
    public readonly uint SearchStart;
    public readonly uint HashStart;
}

/// <summary>
/// The native library's version, as three integers.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct AdaVersionComponents
{
    public readonly int Major;
    public readonly int Minor;
    public readonly int Revision;
}
