using System.Buffers;
using System.Text.Unicode;

namespace Ada.Url.Interop;

/// <summary>
/// UTF-16 to UTF-8 conversion for the convenience overloads.
/// </summary>
/// <remarks>
/// Ada's ABI is UTF-8, so a caller who already has UTF-8 pays nothing. A caller with a
/// <see cref="string"/> pays this transcode, and no amount of cleverness removes it. What we can
/// remove is the allocation, which is what the scratch buffer below is for.
/// </remarks>
internal static class Transcode
{
    /// <summary>
    /// Inputs at or below this many UTF-8 bytes use the stack. Roughly 170 worst case UTF-16
    /// characters. Benchmark W4 tunes this, so do not change it on intuition.
    /// </summary>
    internal const int StackThreshold = 512;

    /// <summary>
    /// Worst case UTF-8 length for a UTF-16 input. A surrogate pair is two chars and four bytes,
    /// so three bytes per char is the bound.
    /// </summary>
    internal static int MaxUtf8Length(int charCount) => charCount * 3;

    /// <summary>
    /// Converts UTF-16 to UTF-8 without ever throwing.
    /// </summary>
    /// <remarks>
    /// Invalid sequences are not replaced, so a lone surrogate returns
    /// <see cref="OperationStatus.InvalidData"/> instead of silently becoming U+FFFD. Callers map
    /// that to a false result. Exceptions must not be control flow on this path.
    /// </remarks>
    internal static OperationStatus Utf16ToUtf8(ReadOnlySpan<char> source, Span<byte> destination, out int written)
        => Utf8.FromUtf16(source, destination, out _, out written, replaceInvalidSequences: false);
}

/// <summary>
/// A UTF-8 scratch buffer that uses the stack for small inputs and the array pool for the rest,
/// and never allocates in steady state.
/// </summary>
/// <remarks>
/// Declared as a ref struct so it cannot outlive the stack frame that owns the stackalloc it may
/// be pointing at.
/// </remarks>
internal ref struct Utf8Scratch
{
    private byte[]? _rented;

    /// <summary>The converted bytes, valid until <see cref="Dispose"/>.</summary>
    public ReadOnlySpan<byte> Bytes { get; private set; }

    /// <summary>True when the source converted cleanly.</summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Converts <paramref name="source"/> into <paramref name="stackBuffer"/> when it fits, and
    /// into a pooled array when it does not.
    /// </summary>
    public Utf8Scratch(ReadOnlySpan<char> source, Span<byte> stackBuffer)
    {
        _rented = null;

        int needed = Transcode.MaxUtf8Length(source.Length);
        Span<byte> target;
        if (needed <= stackBuffer.Length)
        {
            target = stackBuffer;
        }
        else
        {
            _rented = ArrayPool<byte>.Shared.Rent(needed);
            target = _rented;
        }

        Success = Transcode.Utf16ToUtf8(source, target, out int written) == OperationStatus.Done;
        Bytes = Success ? target[..written] : default;
    }

    public void Dispose()
    {
        if (_rented is not null)
        {
            ArrayPool<byte>.Shared.Return(_rented);
            _rented = null;
        }

        Bytes = default;
    }
}
