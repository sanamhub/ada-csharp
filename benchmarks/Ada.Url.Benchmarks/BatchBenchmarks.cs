using System.Text;
using Ada.Url;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace Ada.Url.Benchmarks;

/// <summary>
/// W3. A thousand mixed URLs per invocation, which is closer to what a service actually does than
/// parsing the same string in a loop.
/// </summary>
/// <remarks>
/// <para>
/// Parsing one URL repeatedly measures a warm branch predictor and a hot cache line. Real traffic
/// is a mix, and the mix is where allocation pressure and branch misprediction show up. This is
/// the workload whose numbers should inform a capacity decision.
/// </para>
/// <para>
/// <c>OperationsPerInvoke</c> is 1000, so every figure reads per URL rather than per batch.
/// </para>
/// </remarks>
[MemoryDiagnoser(displayGenColumns: true)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[CategoriesColumn]
[MedianColumn]
public class BatchBenchmarks
{
    private const int BatchSize = 1000;

    private string[] _urls = [];
    private byte[][] _utf8 = [];
    private byte[] _scratch = [];

    [GlobalSetup]
    public void Setup()
    {
        // Fixed seed. A benchmark whose input changes between runs cannot detect a regression.
        var random = new Random(20260826);
        var urls = new List<string>(BatchSize);

        // Proportions chosen to look like traffic rather than to flatter either parser: mostly
        // ordinary URLs, with enough hard cases to keep the slow paths represented.
        Add(urls, 300, i => $"https://example{i % 40}.com/path/{i}");
        Add(urls, 150, i => $"https://example.org/search?q=term{i}&page={i % 20}&sort=desc");
        Add(urls, 100, i => $"https://user{i}:pass{i}@secure.example.net/account");
        Add(urls, 100, i => i % 2 == 0
            ? $"http://192.168.{i % 256}.{(i * 7) % 256}:{8000 + (i % 1000)}/api"
            : $"http://[2001:db8::{i:x}]/api");
        Add(urls, 100, i => $"https://bücher{i % 30}.example/seite");
        Add(urls, 100, i => $"https://example.com/a/b/../c/./d{i}/../e");
        Add(urls, 100, i => i % 2 == 0
            ? $"file:///C:/temp/file{i}.txt"
            : $"custom-scheme://opaque{i}/x");
        Add(urls, 50, i => $"https://example.com/{new string('p', 200 + (i % 300))}?q={new string('v', 100)}");

        // Shuffle, so the benchmark does not measure a run of identical shapes in sequence.
        for (int i = urls.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (urls[i], urls[j]) = (urls[j], urls[i]);
        }

        _urls = [.. urls];
        _utf8 = Array.ConvertAll(_urls, Encoding.UTF8.GetBytes);
        _scratch = new byte[4096];

        static void Add(List<string> target, int count, Func<int, string> make)
        {
            for (int i = 0; i < count; i++)
            {
                target.Add(make(i));
            }
        }
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = BatchSize), BenchmarkCategory("W3 validate")]
    public int SystemUri_Validate()
    {
        int ok = 0;
        foreach (string url in _urls)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                ok++;
            }
        }

        return ok;
    }

    [Benchmark(OperationsPerInvoke = BatchSize), BenchmarkCategory("W3 validate")]
    public int Ada_Validate()
    {
        int ok = 0;
        foreach (byte[] url in _utf8)
        {
            if (AdaUrl.CanParse(url))
            {
                ok++;
            }
        }

        return ok;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = BatchSize), BenchmarkCategory("W3 hostname")]
    public int SystemUri_ExtractHostname()
    {
        // The allow list pattern: parse, take the host, decide. This is the shape most services
        // actually run on every inbound request.
        int total = 0;
        foreach (string url in _urls)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                total += uri.Host.Length;
            }
        }

        return total;
    }

    [Benchmark(OperationsPerInvoke = BatchSize), BenchmarkCategory("W3 hostname")]
    public int Ada_ExtractHostname()
    {
        int total = 0;
        foreach (byte[] url in _utf8)
        {
            if (AdaUrl.TryGetHostname(url, _scratch, out int written))
            {
                total += written;
            }
        }

        return total;
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = BatchSize), BenchmarkCategory("W3 full read")]
    public int SystemUri_ReadAll()
    {
        int total = 0;
        foreach (string url in _urls)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                total += uri.Host.Length + uri.AbsolutePath.Length + uri.Query.Length + uri.Fragment.Length;
            }
        }

        return total;
    }

    [Benchmark(OperationsPerInvoke = BatchSize), BenchmarkCategory("W3 full read")]
    public int Ada_ReadAll()
    {
        int total = 0;
        foreach (byte[] url in _utf8)
        {
            if (!AdaUrl.TryParse(url, out AdaUrl parsed))
            {
                continue;
            }

            using (parsed)
            {
                total += parsed.Hostname.Length + parsed.Pathname.Length
                       + parsed.Search.Length + parsed.Hash.Length;
            }
        }

        return total;
    }
}
