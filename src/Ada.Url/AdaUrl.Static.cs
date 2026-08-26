using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// One shot operations that own no handle.
/// </summary>
/// <remarks>
/// This is the API to reach for first. Most real use is validating or normalising a URL in a
/// request pipeline, and every method here parses, answers, and frees inside the call. Nothing
/// escapes, nothing needs disposing, and with UTF-8 input and a caller supplied buffer nothing
/// allocates either.
/// </remarks>
public ref partial struct AdaUrl
{
    /// <summary>Reports whether UTF-8 input parses as a URL, without building one.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <returns>True when the input is a valid URL.</returns>
    public static unsafe bool CanParse(ReadOnlySpan<byte> utf8)
    {
        fixed (byte* p = utf8)
        {
            return AdaNative.ToBool(AdaNative.CanParse(p, (nuint)utf8.Length));
        }
    }

    /// <summary>Reports whether UTF-8 input parses as a URL against a base URL.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="baseUrl">The base URL to resolve against, as UTF-8 bytes.</param>
    /// <returns>True when the result is a valid URL.</returns>
    public static unsafe bool CanParse(ReadOnlySpan<byte> utf8, ReadOnlySpan<byte> baseUrl)
    {
        fixed (byte* p = utf8)
        fixed (byte* b = baseUrl)
        {
            return AdaNative.ToBool(AdaNative.CanParseWithBase(p, (nuint)utf8.Length, b, (nuint)baseUrl.Length));
        }
    }

    /// <summary>Reports whether UTF-16 input parses as a URL. Costs one transcode.</summary>
    /// <param name="input">The URL.</param>
    /// <returns>True when the input is a valid URL.</returns>
    public static bool CanParse(ReadOnlySpan<char> input)
    {
        Span<byte> stack = stackalloc byte[Transcode.StackThreshold];
        using var scratch = new Utf8Scratch(input, stack);
        return scratch.Success && CanParse(scratch.Bytes);
    }

    /// <summary>Writes the WHATWG normalised form of a URL into a caller supplied buffer.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the input is not a valid URL, or the destination is too small.</returns>
    public static bool TryNormalize(ReadOnlySpan<byte> utf8, Span<byte> destination, out int written)
    {
        if (!TryParse(utf8, out AdaUrl url))
        {
            written = 0;
            return false;
        }

        using (url)
        {
            return CopyOut(url.Href, destination, out written);
        }
    }

    /// <summary>Writes the hostname of a URL into a caller supplied buffer.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the input is not a valid URL, or the destination is too small.</returns>
    /// <remarks>
    /// This is the right primitive for an allow list check. Compare the parsed hostname, never a
    /// prefix of the raw input, and compare it after IDNA has been applied. Both mistakes are
    /// ordinary server side request forgery bypasses.
    /// </remarks>
    public static bool TryGetHostname(ReadOnlySpan<byte> utf8, Span<byte> destination, out int written)
    {
        if (!TryParse(utf8, out AdaUrl url))
        {
            written = 0;
            return false;
        }

        using (url)
        {
            return CopyOut(url.Hostname, destination, out written);
        }
    }

    /// <summary>Writes the origin of a URL into a caller supplied buffer.</summary>
    /// <param name="utf8">The URL, as UTF-8 bytes.</param>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the input is not a valid URL, or the destination is too small.</returns>
    public static bool TryGetOrigin(ReadOnlySpan<byte> utf8, Span<byte> destination, out int written)
    {
        if (!TryParse(utf8, out AdaUrl url))
        {
            written = 0;
            return false;
        }

        using (url)
        {
            return url.TryGetOrigin(destination, out written);
        }
    }

    private static bool CopyOut(ReadOnlySpan<byte> value, Span<byte> destination, out int written)
    {
        if (value.Length > destination.Length)
        {
            written = 0;
            return false;
        }

        value.CopyTo(destination);
        written = value.Length;
        return true;
    }
}
