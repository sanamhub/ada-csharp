using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// A parsed URL, bound to the stack frame that created it.
/// </summary>
/// <remarks>
/// <para>
/// Use this when you need several properties from one URL. For a single question, prefer the
/// one shot statics such as <see cref="CanParse(ReadOnlySpan{byte})"/> and
/// <see cref="TryNormalize"/>, which own no handle at all.
/// </para>
/// <para>
/// Being a ref struct is the safety mechanism. The compiler will not let the handle reach a
/// field, a lambda, an async state machine, or the heap, so its lifetime cannot outrun the
/// enclosing block. Always use a using declaration:
/// </para>
/// <code>
/// using var url = AdaUrl.Parse("https://example.com/a/../b"u8);
/// ReadOnlySpan&lt;byte&gt; path = url.Pathname;   // "/b"
/// </code>
/// <para>
/// Every span this type returns points at native memory and is invalidated by any setter or
/// clear on the same instance. Copy anything you need to keep across a mutation. This is a
/// documented contract rather than an enforced one, because enforcing it would cost the
/// allocation free property the type exists to provide.
/// </para>
/// <para>
/// Not thread safe. Use one instance per thread, or synchronise externally. Concurrent reads of
/// an instance nobody mutates are fine, but a single concurrent setter makes every outstanding
/// span a use after free.
/// </para>
/// </remarks>
public ref partial struct AdaUrl : IDisposable
{
    private nint _handle;

    private AdaUrl(nint handle) => _handle = handle;

    /// <summary>True while this instance owns a live native URL.</summary>
    public readonly bool IsValid => _handle != nint.Zero;

    /// <summary>Parses UTF-8 input.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="url">The parsed URL when this returns true.</param>
    /// <returns>False when the input is not a valid URL. Nothing leaks on failure.</returns>
    public static unsafe bool TryParse(scoped ReadOnlySpan<byte> utf8, out AdaUrl url)
    {
        fixed (byte* p = utf8)
        {
            return Wrap(AdaNative.Parse(p, (nuint)utf8.Length), out url);
        }
    }

    /// <summary>Parses UTF-8 input against a UTF-8 base URL.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="baseUrl">The base URL to resolve against, as UTF-8 bytes.</param>
    /// <param name="url">The parsed URL when this returns true.</param>
    /// <returns>False when the result is not a valid URL. Nothing leaks on failure.</returns>
    public static unsafe bool TryParse(scoped ReadOnlySpan<byte> utf8, scoped ReadOnlySpan<byte> baseUrl, out AdaUrl url)
    {
        fixed (byte* p = utf8)
        fixed (byte* b = baseUrl)
        {
            return Wrap(AdaNative.ParseWithBase(p, (nuint)utf8.Length, b, (nuint)baseUrl.Length), out url);
        }
    }

    /// <summary>Parses UTF-16 input. Costs one transcode and allocates nothing.</summary>
    /// <param name="input">The URL.</param>
    /// <param name="url">The parsed URL when this returns true.</param>
    /// <returns>False when the input is not a valid URL, including when it holds a lone surrogate.</returns>
    public static bool TryParse(scoped ReadOnlySpan<char> input, out AdaUrl url)
    {
        Span<byte> stack = stackalloc byte[Transcode.StackThreshold];
        using var scratch = new Utf8Scratch(input, stack);
        if (!scratch.Success)
        {
            url = default;
            return false;
        }

        return TryParse(scratch.Bytes, out url);
    }

    /// <summary>Parses UTF-8 input, throwing on failure.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <returns>The parsed URL.</returns>
    /// <exception cref="FormatException">The input is not a valid URL.</exception>
    public static AdaUrl Parse(scoped ReadOnlySpan<byte> utf8)
        => TryParse(utf8, out AdaUrl url) ? url : throw new FormatException("Input is not a valid URL.");

    /// <summary>Parses UTF-16 input, throwing on failure.</summary>
    /// <param name="input">The URL.</param>
    /// <returns>The parsed URL.</returns>
    /// <exception cref="FormatException">The input is not a valid URL.</exception>
    public static AdaUrl Parse(scoped ReadOnlySpan<char> input)
        => TryParse(input, out AdaUrl url) ? url : throw new FormatException("Input is not a valid URL.");

    private static bool Wrap(nint handle, out AdaUrl url)
    {
        if (handle == nint.Zero)
        {
            url = default;
            return false;
        }

        // ada_parse hands back a handle even for input it could not parse, so it still has to be
        // freed. Missing this is the most commonly overlooked leak path in a C wrapper.
        if (!AdaNative.ToBool(AdaNative.IsValid(handle)))
        {
            AdaNative.Free(handle);
            url = default;
            return false;
        }

        url = new AdaUrl(handle);
        return true;
    }

    /// <summary>The serialised URL.</summary>
    public readonly ReadOnlySpan<byte> Href => AdaNative.GetHref(_handle).AsSpan();

    /// <summary>The scheme, including the trailing colon.</summary>
    public readonly ReadOnlySpan<byte> Protocol => AdaNative.GetProtocol(_handle).AsSpan();

    /// <summary>The host, including the port when one is present.</summary>
    public readonly ReadOnlySpan<byte> Host => AdaNative.GetHost(_handle).AsSpan();

    /// <summary>The host without the port.</summary>
    public readonly ReadOnlySpan<byte> Hostname => AdaNative.GetHostname(_handle).AsSpan();

    /// <summary>The port, or empty when the URL uses the default port for its scheme.</summary>
    public readonly ReadOnlySpan<byte> Port => AdaNative.GetPort(_handle).AsSpan();

    /// <summary>The path.</summary>
    public readonly ReadOnlySpan<byte> Pathname => AdaNative.GetPathname(_handle).AsSpan();

    /// <summary>The query, including the leading question mark when non empty.</summary>
    public readonly ReadOnlySpan<byte> Search => AdaNative.GetSearch(_handle).AsSpan();

    /// <summary>The fragment, including the leading hash when non empty.</summary>
    public readonly ReadOnlySpan<byte> Hash => AdaNative.GetHash(_handle).AsSpan();

    /// <summary>The username.</summary>
    public readonly ReadOnlySpan<byte> Username => AdaNative.GetUsername(_handle).AsSpan();

    /// <summary>The password.</summary>
    public readonly ReadOnlySpan<byte> Password => AdaNative.GetPassword(_handle).AsSpan();

    /// <summary>True when the URL carries a username or a password.</summary>
    public readonly bool HasCredentials => AdaNative.ToBool(AdaNative.HasCredentials(_handle));

    /// <summary>True when the URL has a hostname.</summary>
    public readonly bool HasHostname => AdaNative.ToBool(AdaNative.HasHostname(_handle));

    /// <summary>True when the hostname is present but empty.</summary>
    public readonly bool HasEmptyHostname => AdaNative.ToBool(AdaNative.HasEmptyHostname(_handle));

    /// <summary>True when an explicit port is present.</summary>
    public readonly bool HasPort => AdaNative.ToBool(AdaNative.HasPort(_handle));

    /// <summary>True when a password is present.</summary>
    public readonly bool HasPassword => AdaNative.ToBool(AdaNative.HasPassword(_handle));

    /// <summary>True when a non empty username is present.</summary>
    public readonly bool HasNonEmptyUsername => AdaNative.ToBool(AdaNative.HasNonEmptyUsername(_handle));

    /// <summary>True when a non empty password is present.</summary>
    public readonly bool HasNonEmptyPassword => AdaNative.ToBool(AdaNative.HasNonEmptyPassword(_handle));

    /// <summary>True when a fragment is present.</summary>
    public readonly bool HasHash => AdaNative.ToBool(AdaNative.HasHash(_handle));

    /// <summary>True when a query is present.</summary>
    public readonly bool HasSearch => AdaNative.ToBool(AdaNative.HasSearch(_handle));

    /// <summary>How the host is represented.</summary>
    public readonly AdaHostType HostType => (AdaHostType)AdaNative.GetHostType(_handle);

    /// <summary>Which special scheme this URL uses, if any.</summary>
    public readonly AdaSchemeType SchemeType => (AdaSchemeType)AdaNative.GetSchemeType(_handle);

    /// <summary>Byte offsets of each component within <see cref="Href"/>.</summary>
    /// <remarks>
    /// Any field can be <see cref="AdaUrlComponents.Omitted"/>, so check before slicing.
    /// </remarks>
    public readonly unsafe AdaUrlComponents Components
    {
        get
        {
            AdaUrlComponentsNative* c = AdaNative.GetComponents(_handle);
            return c is null ? default : new AdaUrlComponents(*c);
        }
    }

    /// <summary>Copies the origin into <paramref name="destination"/>.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the destination is too small.</returns>
    /// <remarks>
    /// The native origin is an owned string. It is released here, so no lifetime escapes.
    /// </remarks>
    public readonly bool TryGetOrigin(Span<byte> destination, out int written)
    {
        AdaOwnedString owned = AdaNative.GetOrigin(_handle);
        try
        {
            ReadOnlySpan<byte> span = owned.AsSpan();
            if (span.Length > destination.Length)
            {
                written = 0;
                return false;
            }

            span.CopyTo(destination);
            written = span.Length;
            return true;
        }
        finally
        {
            AdaNative.FreeOwnedString(owned);
        }
    }

    /// <summary>Replaces the whole URL.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetHref(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetHref(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the host, port included.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetHost(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetHost(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the hostname, leaving the port alone.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetHostname(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetHostname(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the scheme.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetProtocol(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetProtocol(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the path.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetPathname(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetPathname(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the port.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetPort(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetPort(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the username.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetUsername(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetUsername(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the password.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public readonly unsafe bool TrySetPassword(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.SetPassword(_handle, p, (nuint)utf8.Length));
        }
    }

    /// <summary>Replaces the query.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    public readonly unsafe void SetSearch(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            AdaNative.SetSearch(_handle, p, (nuint)utf8.Length);
        }
    }

    /// <summary>Replaces the fragment.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    public readonly unsafe void SetHash(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            AdaNative.SetHash(_handle, p, (nuint)utf8.Length);
        }
    }

    /// <summary>Removes the port.</summary>
    public readonly void ClearPort() => AdaNative.ClearPort(_handle);

    /// <summary>Removes the fragment.</summary>
    public readonly void ClearHash() => AdaNative.ClearHash(_handle);

    /// <summary>Removes the query.</summary>
    public readonly void ClearSearch() => AdaNative.ClearSearch(_handle);

    /// <summary>Releases the native URL. Safe to call more than once.</summary>
    public void Dispose()
    {
        nint handle = _handle;
        _handle = nint.Zero;
        if (handle != nint.Zero)
        {
            AdaNative.Free(handle);
        }
    }
}
