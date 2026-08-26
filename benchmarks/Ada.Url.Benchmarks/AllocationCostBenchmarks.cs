using System.Text;
using Ada.Url;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Ada.Url.Benchmarks;

/// <summary>
/// W4. What the parse path actually spends its time on.
/// </summary>
/// <remarks>
/// <para>
/// A sustained run over ten million URLs put validation at about six million a second and a full
/// parse at about two million. The gap is not parsing and it is not the P/Invoke boundary, which
/// costs a couple of nanoseconds per call. <c>ada_parse</c> heap allocates a URL object that
/// <c>ada_free</c> then releases, and that pair is roughly two thirds of the cost of a parse.
/// </para>
/// <para>
/// That matters when choosing an API. Code that only answers "is this a URL" should call
/// <see cref="AdaUrl.CanParse(ReadOnlySpan{byte})"/> and will run about three times faster than
/// code that parses and throws the result away.
/// </para>
/// <para>
/// It is measured here rather than left as a note, so a future change to the binding or to
/// upstream Ada that removes the allocation shows up as a number instead of going unnoticed.
/// <c>ada_c.h</c> offers no way to parse into caller supplied storage today, so this is an
/// upstream limit rather than something the binding can work around.
/// </para>
/// <para>
/// The working set is a parameter because a benchmark that parses one URL in a loop measures a
/// hot cache line. Sustained throughput held between 1.8 and 2.0 million URLs a second from a six
/// kibibyte working set to a sixty mebibyte one, so this is expected to stay flat. A run where it
/// does not is worth investigating.
/// </para>
/// </remarks>
[MemoryDiagnoser(displayGenColumns: true)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[CategoriesColumn]
[MedianColumn]
public class AllocationCostBenchmarks
{
    private const int Batch = 1000;

    /// <summary>Distinct URLs held live, to vary how much of the working set fits in cache.</summary>
    [Params(100, 10_000, 200_000)]
    public int WorkingSet { get; set; }

    private byte[][] _utf8 = [];
    private int _cursor;

    [GlobalSetup]
    public void Setup()
    {
        // Fixed seed. A benchmark whose input changes between runs cannot detect a regression.
        var random = new Random(20260826);

        string[] hosts =
        [
            "example.com", "www.example.org", "cdn.example.net", "api.service.io",
            "shop.example.co.uk", "images.example.com", "a.very.long.subdomain.chain.example.com",
            "192.168.1.1", "xn--bcher-kva.example", "deep.nested.host.name.example.org",
        ];
        string[] schemes = ["https", "http", "https", "https", "ws", "ftp"];
        string[] paths =
        [
            "/", "/index.html", "/a/b/c/d/e/f", "/products/12345/reviews", "/search",
            "/a/./b/../c/d", "/%E4%BD%A0%E5%A5%BD/path", "/very/deep/nesting/of/segments/here/ok",
        ];
        string[] queries = ["", "?q=1", "?a=1&b=2&c=3", "?query=hello+world&lang=en&page=42"];
        string[] fragments = ["", "#top", "#section-4", "#a/b"];

        _utf8 = new byte[WorkingSet][];
        for (int i = 0; i < WorkingSet; i++)
        {
            string url = string.Concat(
                schemes[random.Next(schemes.Length)], "://",
                hosts[random.Next(hosts.Length)],
                random.Next(6) == 0 ? ":8443" : string.Empty,
                paths[random.Next(paths.Length)],
                queries[random.Next(queries.Length)],
                fragments[random.Next(fragments.Length)]);

            _utf8[i] = Encoding.UTF8.GetBytes(url);
        }

        _cursor = 0;
    }

    /// <summary>Validation only. Nothing is allocated natively and nothing is returned.</summary>
    [Benchmark(Baseline = true, OperationsPerInvoke = Batch), BenchmarkCategory("W4")]
    public int CanParse()
    {
        int valid = 0;
        for (int i = 0; i < Batch; i++)
        {
            if (AdaUrl.CanParse(Next()))
            {
                valid++;
            }
        }

        return valid;
    }

    /// <summary>
    /// The same parse, but keeping the URL object. The difference against <see cref="CanParse"/>
    /// is the native allocation and free.
    /// </summary>
    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("W4")]
    public int ParseAndDispose()
    {
        int valid = 0;
        for (int i = 0; i < Batch; i++)
        {
            if (AdaUrl.TryParse(Next(), out AdaUrl url))
            {
                using (url)
                {
                    valid++;
                }
            }
        }

        return valid;
    }

    /// <summary>Reading a component on top of the parse, to show how little the reads cost.</summary>
    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("W4")]
    public int ParseAndReadHostname()
    {
        int total = 0;
        for (int i = 0; i < Batch; i++)
        {
            if (AdaUrl.TryParse(Next(), out AdaUrl url))
            {
                using (url)
                {
                    total += url.Hostname.Length;
                }
            }
        }

        return total;
    }

    /// <summary>Five components rather than one, for the same reason.</summary>
    [Benchmark(OperationsPerInvoke = Batch), BenchmarkCategory("W4")]
    public int ParseAndReadFive()
    {
        int total = 0;
        for (int i = 0; i < Batch; i++)
        {
            if (AdaUrl.TryParse(Next(), out AdaUrl url))
            {
                using (url)
                {
                    total += url.Href.Length + url.Hostname.Length + url.Pathname.Length
                           + url.Search.Length + url.Protocol.Length;
                }
            }
        }

        return total;
    }

    /// <summary>
    /// Walks the corpus rather than restarting each invocation, so a small batch against a large
    /// working set still touches all of it instead of the same first thousand entries.
    /// </summary>
    private ReadOnlySpan<byte> Next()
    {
        int index = _cursor;
        _cursor = index + 1 == _utf8.Length ? 0 : index + 1;
        return _utf8[index];
    }
}
