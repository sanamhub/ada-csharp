using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Ada.Url.Benchmarks;

internal static class Program
{
    private static void Main(string[] args)
    {
        // WithOptions(JoinSummary) prints one table across all classes, which makes the tiers
        // comparable at a glance instead of scattered over several summaries.
        //
        // The Baseline column is not on by default, and without it the export says which row is
        // the baseline only by printing its ratio as 1.00. A measured row that happens to land
        // within half a percent of the baseline prints 1.00 too, so anything reading the export
        // cannot tell them apart. That happened to two Windows x64 rows in the 0.1.0 run.
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(
                args,
                DefaultConfig.Instance
                    .WithOptions(ConfigOptions.JoinSummary)
                    .AddColumn(BaselineColumn.Default));
    }
}
