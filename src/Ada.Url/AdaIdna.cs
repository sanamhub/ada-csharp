using System.Text;
using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// Converts domain names between Unicode and ASCII, following UTS-46.
/// </summary>
/// <remarks>
/// <para>
/// This matters for security, not just display. Two domains that look identical to a person can
/// map to entirely different ASCII, so an allow list has to compare the ASCII form. Comparing at
/// the Unicode level is an ordinary server side request forgery bypass.
/// </para>
/// <para>
/// Both native functions return memory the caller owns. Every method here frees it before
/// returning, so no lifetime escapes.
/// </para>
/// </remarks>
public static class AdaIdna
{
    /// <summary>Converts a domain to its ASCII form, writing into a caller supplied buffer.</summary>
    /// <param name="utf8Domain">The domain, as UTF-8 bytes.</param>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the domain cannot be converted, or the destination is too small.</returns>
    /// <example>
    /// <c>"Bücher.example"</c> becomes <c>"xn--bcher-kva.example"</c>.
    /// </example>
    public static unsafe bool TryToAscii(ReadOnlySpan<byte> utf8Domain, Span<byte> destination, out int written)
    {
        fixed (byte* p = utf8Domain)
        {
            AdaOwnedString owned = AdaNative.IdnaToAscii(p, (nuint)utf8Domain.Length);
            return CopyAndRelease(owned, destination, out written);
        }
    }

    /// <summary>Converts a domain to its Unicode form, writing into a caller supplied buffer.</summary>
    /// <param name="utf8Domain">The domain, as UTF-8 bytes.</param>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the domain cannot be converted, or the destination is too small.</returns>
    /// <example>
    /// <c>"xn--bcher-kva.example"</c> becomes <c>"bücher.example"</c>.
    /// </example>
    public static unsafe bool TryToUnicode(ReadOnlySpan<byte> utf8Domain, Span<byte> destination, out int written)
    {
        fixed (byte* p = utf8Domain)
        {
            AdaOwnedString owned = AdaNative.IdnaToUnicode(p, (nuint)utf8Domain.Length);
            return CopyAndRelease(owned, destination, out written);
        }
    }

    /// <summary>Converts a domain to its ASCII form.</summary>
    /// <param name="domain">The domain.</param>
    /// <returns>The ASCII form, or an empty string when the domain cannot be converted.</returns>
    /// <remarks>Allocates the result. Use the span overload on a hot path.</remarks>
    public static unsafe string ToAscii(string domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        byte[] utf8 = Encoding.UTF8.GetBytes(domain);
        fixed (byte* p = utf8)
        {
            AdaOwnedString owned = AdaNative.IdnaToAscii(p, (nuint)utf8.Length);
            try
            {
                return Encoding.UTF8.GetString(owned.AsSpan());
            }
            finally
            {
                AdaNative.FreeOwnedString(owned);
            }
        }
    }

    /// <summary>Converts a domain to its Unicode form.</summary>
    /// <param name="domain">The domain, normally in its ASCII form.</param>
    /// <returns>The Unicode form, or an empty string when the domain cannot be converted.</returns>
    /// <remarks>Allocates the result. Use the span overload on a hot path.</remarks>
    public static unsafe string ToUnicode(string domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        byte[] utf8 = Encoding.UTF8.GetBytes(domain);
        fixed (byte* p = utf8)
        {
            AdaOwnedString owned = AdaNative.IdnaToUnicode(p, (nuint)utf8.Length);
            try
            {
                return Encoding.UTF8.GetString(owned.AsSpan());
            }
            finally
            {
                AdaNative.FreeOwnedString(owned);
            }
        }
    }

    private static bool CopyAndRelease(AdaOwnedString owned, Span<byte> destination, out int written)
    {
        try
        {
            ReadOnlySpan<byte> span = owned.AsSpan();
            if (span.IsEmpty || span.Length > destination.Length)
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
}
