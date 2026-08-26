using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// Byte offsets of each URL component within the serialised href.
/// </summary>
/// <remarks>
/// <para>
/// Every field can hold <see cref="Omitted"/>, meaning the component is absent. That value is
/// <c>uint.MaxValue</c>, so casting an omitted field to <see cref="int"/> gives -1 and an out of
/// range slice. Check with <see cref="IsPresent"/> before using any of them.
/// </para>
/// <para>
/// <see cref="HostStart"/> points at the <c>@</c> rather than at the host when the URL carries
/// credentials, so slicing <c>href[HostStart..HostEnd]</c> yields <c>@example.com</c> for
/// <c>https://user:pw@example.com/</c>. Without credentials the same slice is correct. Use
/// <c>AdaUrl.Hostname</c> unless you specifically want the offsets.
/// </para>
/// <para>
/// These are offsets, not a description of the whole URL. Nothing records where the path ends,
/// so two URLs that share a scheme and host compare equal here however much their paths differ.
/// Component equality is not URL equality; compare <c>AdaUrl.Href</c> for that.
/// </para>
/// </remarks>
public readonly struct AdaUrlComponents : IEquatable<AdaUrlComponents>
{
    /// <summary>The value a field carries when its component is absent.</summary>
    public const uint Omitted = 0xFFFFFFFFu;

    internal AdaUrlComponents(AdaUrlComponentsNative native)
    {
        ProtocolEnd = native.ProtocolEnd;
        UsernameEnd = native.UsernameEnd;
        HostStart = native.HostStart;
        HostEnd = native.HostEnd;
        Port = native.Port;
        PathnameStart = native.PathnameStart;
        SearchStart = native.SearchStart;
        HashStart = native.HashStart;
    }

    /// <summary>Offset just past the scheme and its colon.</summary>
    public uint ProtocolEnd { get; }

    /// <summary>Offset just past the username.</summary>
    public uint UsernameEnd { get; }

    /// <summary>Offset where the host begins.</summary>
    public uint HostStart { get; }

    /// <summary>Offset just past the host.</summary>
    public uint HostEnd { get; }

    /// <summary>The port as a number, or <see cref="Omitted"/> when the default port applies.</summary>
    public uint Port { get; }

    /// <summary>Offset where the path begins.</summary>
    public uint PathnameStart { get; }

    /// <summary>Offset where the query begins, or <see cref="Omitted"/> when there is none.</summary>
    public uint SearchStart { get; }

    /// <summary>Offset where the fragment begins, or <see cref="Omitted"/> when there is none.</summary>
    public uint HashStart { get; }

    /// <summary>True when a field holds a real offset rather than the absent sentinel.</summary>
    /// <param name="value">One of this struct's fields.</param>
    /// <returns>True when the component is present.</returns>
    public static bool IsPresent(uint value) => value != Omitted;

    /// <inheritdoc/>
    public bool Equals(AdaUrlComponents other)
        => ProtocolEnd == other.ProtocolEnd
        && UsernameEnd == other.UsernameEnd
        && HostStart == other.HostStart
        && HostEnd == other.HostEnd
        && Port == other.Port
        && PathnameStart == other.PathnameStart
        && SearchStart == other.SearchStart
        && HashStart == other.HashStart;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AdaUrlComponents other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(ProtocolEnd);
        hash.Add(UsernameEnd);
        hash.Add(HostStart);
        hash.Add(HostEnd);
        hash.Add(Port);
        hash.Add(PathnameStart);
        hash.Add(SearchStart);
        hash.Add(HashStart);
        return hash.ToHashCode();
    }

    /// <summary>Compares two instances for equality.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when the two are equal.</returns>
    public static bool operator ==(AdaUrlComponents left, AdaUrlComponents right) => left.Equals(right);

    /// <summary>Compares two instances for inequality.</summary>
    /// <param name="left">Left operand.</param>
    /// <param name="right">Right operand.</param>
    /// <returns>True when the two differ.</returns>
    public static bool operator !=(AdaUrlComponents left, AdaUrlComponents right) => !left.Equals(right);
}

/// <summary>
/// How a URL's host is represented.
/// </summary>
public enum AdaHostType : byte
{
    /// <summary>A registered domain name.</summary>
    Domain = 0,

    /// <summary>An IPv4 address.</summary>
    IPv4 = 1,

    /// <summary>An IPv6 address.</summary>
    IPv6 = 2,
}

/// <summary>
/// Which scheme a URL uses. The values below <see cref="NotSpecial"/> are the WHATWG special
/// schemes, which get default ports and path normalisation.
/// </summary>
public enum AdaSchemeType : byte
{
    /// <summary>The http scheme.</summary>
    Http = 0,

    /// <summary>The not special marker. Any scheme outside the special list.</summary>
    NotSpecial = 1,

    /// <summary>The https scheme.</summary>
    Https = 2,

    /// <summary>The ws scheme.</summary>
    Ws = 3,

    /// <summary>The ftp scheme.</summary>
    Ftp = 4,

    /// <summary>The wss scheme.</summary>
    Wss = 5,

    /// <summary>The file scheme.</summary>
    File = 6,
}
