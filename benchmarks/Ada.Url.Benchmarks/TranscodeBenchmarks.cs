using System.Text;
using Ada.Url;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Ada.Url.Benchmarks;

/// <summary>
/// W4. What a UTF-16 caller pays, measured across input lengths.
/// </summary>
/// <remarks>
/// <para>
/// Ada's API is UTF-8, so a caller holding a <see cref="string"/> pays a transcode that no amount
/// of interop tuning removes. This exists to put a number on it rather than describe it as a
/// documented cost and leave the reader guessing.
/// </para>
/// <para>
/// It is also what sets the stackalloc threshold. Below the threshold the scratch buffer is on
/// the stack and allocates nothing; above it the buffer comes from the array pool. The constant
/// in the library should be justified by this measurement rather than chosen.
/// </para>
/// </remarks>
[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.Declared)]
[MedianColumn]
public class TranscodeBenchmarks
{
    /// <summary>
    /// Input length in characters. 170 characters is roughly the 512 byte stackalloc threshold at
    /// worst case expansion, so the pair either side of it is where the pooled path takes over.
    /// </summary>
    [Params(16, 64, 170, 256, 1024, 4096)]
    public int Length { get; set; }

    private string _ascii = string.Empty;
    private string _mixed = string.Empty;
    private byte[] _asciiUtf8 = [];

    [GlobalSetup]
    public void Setup()
    {
        const string Prefix = "https://example.com/";
        int pad = Math.Max(1, Length - Prefix.Length);

        _ascii = Prefix + new string('a', pad);

        // Non ASCII costs more to transcode, and a URL with an internationalised host or a
        // percent decoded query is not an unusual input.
        _mixed = Prefix + string.Concat(Enumerable.Repeat("é", pad / 2));

        _asciiUtf8 = Encoding.UTF8.GetBytes(_ascii);
    }

    /// <summary>The floor. UTF-8 in, no transcode at all.</summary>
    [Benchmark(Baseline = true)]
    public bool Utf8_NoTranscode() => AdaUrl.CanParse(_asciiUtf8);

    /// <summary>ASCII UTF-16 in. One byte per character, so the cheapest transcode.</summary>
    [Benchmark]
    public bool Utf16_Ascii() => AdaUrl.CanParse(_ascii.AsSpan());

    /// <summary>Non ASCII UTF-16 in. Two bytes per character.</summary>
    [Benchmark]
    public bool Utf16_NonAscii() => AdaUrl.CanParse(_mixed.AsSpan());

    /// <summary>
    /// What a caller pays converting to UTF-8 themselves before calling, which is what a service
    /// holding strings would have to do to reach the allocation free path.
    /// </summary>
    [Benchmark]
    public bool Utf16_CallerTranscodes()
    {
        int max = Encoding.UTF8.GetMaxByteCount(_ascii.Length);
        byte[] rented = System.Buffers.ArrayPool<byte>.Shared.Rent(max);
        try
        {
            int written = Encoding.UTF8.GetBytes(_ascii, rented);
            return AdaUrl.CanParse(rented.AsSpan(0, written));
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        }
    }
}
