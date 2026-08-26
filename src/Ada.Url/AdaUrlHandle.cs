using System.Runtime.InteropServices;
using System.Text;
using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// A parsed URL that can be stored in a field, cached, or used across an await.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="AdaUrl"/> is a ref struct, so the compiler will not let it reach a field, a lambda,
/// an async state machine, or the heap. That is what makes it free. When a URL genuinely has to
/// outlive a stack frame, this type is the way to do it, and the cost is visible: one small
/// allocation and a finalizer registration per URL.
/// </para>
/// <para>
/// The finalizer means a dropped instance is eventually cleaned up rather than leaked, which is
/// the same guarantee goada gets from runtime.AddCleanup. Dispose it anyway. Waiting for a
/// finalizer means holding native memory until the next collection notices.
/// </para>
/// <para>
/// Every span returned here points at native memory. It stays valid while the handle is alive and
/// unmutated, and is invalidated by any setter. Reads take a reference count on the handle for
/// their duration, so the native URL cannot be freed underneath a call in progress, but that does
/// not extend the life of a span after the call returns. Copy anything you need to keep.
/// </para>
/// <para>
/// Not thread safe for mutation. Concurrent reads of an instance nobody mutates are fine.
/// </para>
/// </remarks>
public sealed unsafe class AdaUrlHandle : SafeHandle
{
    private AdaUrlHandle(nint handle)
        : base(nint.Zero, ownsHandle: true)
    {
        SetHandle(handle);
    }

    /// <inheritdoc/>
    public override bool IsInvalid => handle == nint.Zero;

    /// <summary>Parses UTF-8 input into a handle that can be stored.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="url">The parsed URL when this returns true.</param>
    /// <returns>False when the input is not a valid URL. Nothing leaks on failure.</returns>
    public static bool TryParse(scoped ReadOnlySpan<byte> utf8, out AdaUrlHandle? url)
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
    public static bool TryParse(scoped ReadOnlySpan<byte> utf8, scoped ReadOnlySpan<byte> baseUrl, out AdaUrlHandle? url)
    {
        fixed (byte* p = utf8)
        fixed (byte* b = baseUrl)
        {
            return Wrap(AdaNative.ParseWithBase(p, (nuint)utf8.Length, b, (nuint)baseUrl.Length), out url);
        }
    }

    /// <summary>Parses UTF-16 input. Costs one transcode.</summary>
    /// <param name="input">The URL.</param>
    /// <param name="url">The parsed URL when this returns true.</param>
    /// <returns>False when the input is not a valid URL, including when it holds a lone surrogate.</returns>
    public static bool TryParse(scoped ReadOnlySpan<char> input, out AdaUrlHandle? url)
    {
        Span<byte> stack = stackalloc byte[Transcode.StackThreshold];
        using var scratch = new Utf8Scratch(input, stack);
        if (!scratch.Success)
        {
            url = null;
            return false;
        }

        return TryParse(scratch.Bytes, out url);
    }

    /// <summary>Parses UTF-8 input, throwing on failure.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <returns>The parsed URL.</returns>
    /// <exception cref="FormatException">The input is not a valid URL.</exception>
    public static AdaUrlHandle Parse(scoped ReadOnlySpan<byte> utf8)
        => TryParse(utf8, out AdaUrlHandle? url) && url is not null
            ? url
            : throw new FormatException("Input is not a valid URL.");

    private static bool Wrap(nint raw, out AdaUrlHandle? url)
    {
        if (raw == nint.Zero)
        {
            url = null;
            return false;
        }

        // ada_parse returns a handle even for input it could not parse, so a failed parse still
        // has to free it.
        if (!AdaNative.ToBool(AdaNative.IsValid(raw)))
        {
            AdaNative.Free(raw);
            url = null;
            return false;
        }

        url = new AdaUrlHandle(raw);
        return true;
    }

    /// <summary>The serialised URL.</summary>
    /// <returns>The href as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetHref() => Read(&AdaNative.GetHref);

    /// <summary>The host without the port.</summary>
    /// <returns>The hostname as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetHostname() => Read(&AdaNative.GetHostname);

    /// <summary>The host, including the port when one is present.</summary>
    /// <returns>The host as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetHost() => Read(&AdaNative.GetHost);

    /// <summary>The scheme, including the trailing colon.</summary>
    /// <returns>The protocol as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetProtocol() => Read(&AdaNative.GetProtocol);

    /// <summary>The path.</summary>
    /// <returns>The pathname as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetPathname() => Read(&AdaNative.GetPathname);

    /// <summary>The query, including the leading question mark when non empty.</summary>
    /// <returns>The search as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetSearch() => Read(&AdaNative.GetSearch);

    /// <summary>The fragment, including the leading hash when non empty.</summary>
    /// <returns>The hash as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetHash() => Read(&AdaNative.GetHash);

    /// <summary>The port, or empty when the URL uses the default port for its scheme.</summary>
    /// <returns>The port as UTF-8 bytes, borrowed from native memory.</returns>
    public ReadOnlySpan<byte> GetPort() => Read(&AdaNative.GetPort);

    /// <summary>True when the URL carries a username or a password.</summary>
    public bool HasCredentials => ReadFlag(&AdaNative.HasCredentials);

    /// <summary>True when an explicit port is present.</summary>
    public bool HasPort => ReadFlag(&AdaNative.HasPort);

    /// <summary>True when a query is present.</summary>
    public bool HasSearch => ReadFlag(&AdaNative.HasSearch);

    /// <summary>The serialised URL as a string.</summary>
    /// <returns>The href. Allocates, unlike <see cref="GetHref"/>.</returns>
    public override string ToString() => Encoding.UTF8.GetString(GetHref());

    /// <summary>Replaces the host. Invalidates every span previously read from this instance.</summary>
    /// <param name="utf8">The new value, as UTF-8 bytes.</param>
    /// <returns>False when the value was rejected.</returns>
    public bool TrySetHost(scoped ReadOnlySpan<byte> utf8)
    {
        bool added = false;
        try
        {
            DangerousAddRef(ref added);
            fixed (byte* p = utf8)
            {
                return AdaNative.ToBool(AdaNative.SetHost(handle, p, (nuint)utf8.Length));
            }
        }
        finally
        {
            if (added)
            {
                DangerousRelease();
            }
        }
    }

    /// <summary>Copies the origin into <paramref name="destination"/>.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the destination is too small.</returns>
    public bool TryGetOrigin(Span<byte> destination, out int written)
    {
        bool added = false;
        try
        {
            DangerousAddRef(ref added);
            AdaOwnedString owned = AdaNative.GetOrigin(handle);
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
        finally
        {
            if (added)
            {
                DangerousRelease();
            }
        }
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        AdaNative.Free(handle);
        return true;
    }

    /// <summary>
    /// Runs a native getter with the handle reference counted for the duration of the call.
    /// </summary>
    /// <remarks>
    /// This is the analogue of goada's runtime.KeepAlive. Without it, nothing stops the finalizer
    /// running between reading the handle and the native call using it.
    /// </remarks>
    private ReadOnlySpan<byte> Read(delegate*<nint, AdaString> getter)
    {
        bool added = false;
        try
        {
            DangerousAddRef(ref added);
            return getter(handle).AsSpan();
        }
        finally
        {
            if (added)
            {
                DangerousRelease();
            }
        }
    }

    private bool ReadFlag(delegate*<nint, byte> predicate)
    {
        bool added = false;
        try
        {
            DangerousAddRef(ref added);
            return AdaNative.ToBool(predicate(handle));
        }
        finally
        {
            if (added)
            {
                DangerousRelease();
            }
        }
    }
}
