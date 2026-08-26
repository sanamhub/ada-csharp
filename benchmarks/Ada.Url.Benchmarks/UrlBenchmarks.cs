using System.Text;
using Ada.Url;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Ada.Url.Benchmarks;

/// <summary>
/// Compares this wrapper against System.Uri.
/// </summary>
/// <remarks>
/// <para>
/// Results are split into three tiers, and the zero byte number only ever refers to T1.
/// </para>
/// <list type="bullet">
/// <item><description>T1: UTF-8 in, span out. The target is zero bytes allocated.</description></item>
/// <item><description>T2: UTF-8 in, string out. Allocates exactly one string.</description></item>
/// <item><description>T3: string in, string out. The fair comparison against System.Uri.</description></item>
/// </list>
/// <para>
/// Publishing only T1 while callers write T3 code would mislead. Publishing only T3 hides what
/// the library actually does. Both go in the results.
/// </para>
/// <para>
/// The two parsers do not implement the same specification. Ada follows WHATWG, System.Uri
/// follows RFC 3986 and 3987 plus a decade of .NET behaviour. Speed is only half the comparison,
/// so read this next to docs/system-uri-differences.md.
/// </para>
/// </remarks>
[MemoryDiagnoser(displayGenColumns: true)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
// Without this, every benchmark in the class is one group and only one method in the whole
// class may carry Baseline = true. W1 and W2 each need their own System.Uri baseline, so the
// groups have to follow the categories.
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
[MedianColumn]
public class UrlBenchmarks
{
    // W1: a plain URL with nothing unusual in it.
    private const string BasicUrl = "https://example.com/path";

    // W2: credentials, a non default port, dot segments, percent encoding, an internationalised
    // host, and a heavy query. This is where the two parsers diverge most.
    private const string ComplexUrl =
        "https://user:p%40ss@sub.dømain.example.co.uk:8443/a/../b/./c%2Fd/e%20f" +
        "?q=hello+world&filter[]=1&filter[]=2&token=%E2%9C%93&redirect=https%3A%2F%2Fother.example%2Fx" +
        "#section-2%20anchor";

    private byte[] _basicUtf8 = [];
    private byte[] _complexUtf8 = [];
    private byte[] _scratch = [];

    [GlobalSetup]
    public void Setup()
    {
        // Pre-transcode so T1 measures parsing rather than setup.
        _basicUtf8 = Encoding.UTF8.GetBytes(BasicUrl);
        _complexUtf8 = Encoding.UTF8.GetBytes(ComplexUrl);
        _scratch = new byte[1024];
    }

    // -------------------------------------------------------------------------------------
    // W1, basic URL
    // -------------------------------------------------------------------------------------

    [Benchmark(Baseline = true), BenchmarkCategory("W1")]
    public int SystemUri_Basic()
    {
        var uri = new Uri(BasicUrl);
        return uri.Host.Length + uri.AbsolutePath.Length + uri.Query.Length;
    }

    [Benchmark, BenchmarkCategory("W1")]
    public bool Ada_Basic_CanParse() => AdaUrl.CanParse(_basicUtf8);

    [Benchmark, BenchmarkCategory("W1")]
    public int Ada_Basic_T1_SpanIn_SpanOut()
    {
        if (!AdaUrl.TryParse(_basicUtf8, out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return url.Hostname.Length + url.Pathname.Length + url.Search.Length;
        }
    }

    [Benchmark, BenchmarkCategory("W1")]
    public int Ada_Basic_T1_ReadEveryComponent()
    {
        if (!AdaUrl.TryParse(_basicUtf8, out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return url.Href.Length + url.Protocol.Length + url.Host.Length + url.Hostname.Length
                 + url.Port.Length + url.Pathname.Length + url.Search.Length + url.Hash.Length
                 + url.Username.Length + url.Password.Length;
        }
    }

    [Benchmark, BenchmarkCategory("W1")]
    public int Ada_Basic_T2_SpanIn_StringOut()
    {
        if (!AdaUrl.TryParse(_basicUtf8, out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return Encoding.UTF8.GetString(url.Href).Length;
        }
    }

    [Benchmark, BenchmarkCategory("W1")]
    public int Ada_Basic_T3_StringIn_StringOut()
    {
        if (!AdaUrl.TryParse(BasicUrl.AsSpan(), out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return Encoding.UTF8.GetString(url.Href).Length;
        }
    }

    // -------------------------------------------------------------------------------------
    // W2, complex URL
    // -------------------------------------------------------------------------------------

    [Benchmark(Baseline = true), BenchmarkCategory("W2")]
    public int SystemUri_Complex()
    {
        var uri = new Uri(ComplexUrl);
        return uri.Host.Length + uri.AbsolutePath.Length + uri.Query.Length + uri.Fragment.Length;
    }

    [Benchmark, BenchmarkCategory("W2")]
    public int Ada_Complex_T1_SpanIn_SpanOut()
    {
        if (!AdaUrl.TryParse(_complexUtf8, out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return url.Hostname.Length + url.Pathname.Length + url.Search.Length + url.Hash.Length;
        }
    }

    [Benchmark, BenchmarkCategory("W2")]
    public int Ada_Complex_T1_Normalize()
    {
        return AdaUrl.TryNormalize(_complexUtf8, _scratch, out int written) ? written : 0;
    }

    [Benchmark, BenchmarkCategory("W2")]
    public int Ada_Complex_T3_StringIn_StringOut()
    {
        if (!AdaUrl.TryParse(ComplexUrl.AsSpan(), out AdaUrl url))
        {
            return 0;
        }

        using (url)
        {
            return Encoding.UTF8.GetString(url.Href).Length;
        }
    }
}
