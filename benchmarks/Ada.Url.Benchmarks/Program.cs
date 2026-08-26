using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Ada.Url.Benchmarks;

internal static class Program
{
    private static void Main(string[] args)
    {
        // WithOptions(JoinSummary) prints one table across all classes, which makes the tiers
        // comparable at a glance instead of scattered over several summaries.
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args, DefaultConfig.Instance.WithOptions(ConfigOptions.JoinSummary));
    }
}
