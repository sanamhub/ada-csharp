using System.Globalization;
using System.Text;
using Xunit;

namespace Ada.Url.Tests.Conformance;

/// <summary>
/// Generates the report of where this library and <see cref="Uri"/> disagree.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Uri"/> is never used as an oracle here. It implements RFC 3986 and 3987 plus a
/// decade of .NET specific behaviour, and Ada implements WHATWG. They disagree on real inputs by
/// design, so a divergence is information rather than a failure.
/// </para>
/// <para>
/// The output answers the question every prospective user actually has, which is not "is it
/// faster" but "what changes if I switch". A benchmark without this alongside it is only half an
/// argument.
/// </para>
/// </remarks>
[Trait("Category", "Conformance")]
public class SystemUriDifferenceReport
{
    /// <summary>
    /// Walks the WHATWG corpus, records every disagreement, and writes the report.
    /// </summary>
    /// <remarks>
    /// Always passes. It is a generator, not an assertion. The only thing that could fail it is
    /// the corpus failing to load, which is covered separately.
    /// </remarks>
    [Fact]
    public void Generate()
    {
        var bothParsed = new List<Divergence>();
        int adaOnly = 0;
        int uriOnly = 0;
        int agreed = 0;
        int considered = 0;

        foreach (UrlCase test in WhatwgCorpus.Parsing)
        {
            // Relative references need a base, and Uri handles that through a different
            // constructor with different rules. Comparing them would measure the comparison
            // rather than the parsers.
            if (test.Base is not null)
            {
                continue;
            }

            considered++;

            byte[] utf8 = Encoding.UTF8.GetBytes(test.Input);
            bool adaOk = AdaUrl.TryParse(utf8, out AdaUrl url);
            bool uriOk = Uri.TryCreate(test.Input, UriKind.Absolute, out Uri? uri);

            using (url)
            {
                if (adaOk && !uriOk)
                {
                    adaOnly++;
                    continue;
                }

                if (!adaOk && uriOk)
                {
                    uriOnly++;
                    continue;
                }

                if (!adaOk)
                {
                    agreed++;
                    continue;
                }

                string adaHref = Encoding.UTF8.GetString(url.Href);
                string uriHref = uri!.AbsoluteUri;

                if (string.Equals(adaHref, uriHref, StringComparison.Ordinal))
                {
                    agreed++;
                }
                else
                {
                    bothParsed.Add(new Divergence(test.Input, adaHref, uriHref));
                }
            }
        }

        string path = Path.Combine(AppContext.BaseDirectory, "system-uri-differences.md");
        File.WriteAllText(path, Render(considered, agreed, adaOnly, uriOnly, bothParsed), Encoding.UTF8);

        // Surfaced in the test output so a CI run reports the shape without opening the artifact.
        Assert.True(
            considered > 0,
            $"considered {considered}, agreed {agreed}, ada only {adaOnly}, uri only {uriOnly}, " +
            $"different serialisation {bothParsed.Count}");
    }

    private static string Render(int considered, int agreed, int adaOnly, int uriOnly, List<Divergence> diffs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Where Ada.Url and System.Uri disagree");
        sb.AppendLine();
        sb.AppendLine("Generated from the vendored web-platform-tests URL corpus. Do not edit by hand.");
        sb.AppendLine();
        sb.AppendLine("`System.Uri` is not wrong here, and neither is Ada. They implement different");
        sb.AppendLine("specifications. Ada follows the WHATWG URL Standard, which is what browsers, Node, Go");
        sb.AppendLine("and Python implement. `System.Uri` follows RFC 3986 and 3987 plus a decade of .NET");
        sb.AppendLine("specific behaviour. This file exists so the difference is a decision you make rather");
        sb.AppendLine("than a surprise you discover in production.");
        sb.AppendLine();
        sb.AppendLine("Only absolute URLs are compared. Relative references need a base, and `System.Uri`");
        sb.AppendLine("resolves those through a different constructor with different rules, so comparing");
        sb.AppendLine("them would measure the comparison rather than the parsers.");
        sb.AppendLine();
        sb.AppendLine("## Summary");
        sb.AppendLine();
        sb.AppendLine("| | Count |");
        sb.AppendLine("| --- | ---: |");
        sb.AppendLine(Row("Absolute cases compared", considered));
        sb.AppendLine(Row("Same result", agreed));
        sb.AppendLine(Row("Accepted by Ada, rejected by System.Uri", adaOnly));
        sb.AppendLine(Row("Rejected by Ada, accepted by System.Uri", uriOnly));
        sb.AppendLine(Row("Both parsed, different serialisation", diffs.Count));
        sb.AppendLine();

        if (uriOnly > 0)
        {
            sb.AppendLine("The third row is the one that matters for security. An input `System.Uri` accepts");
            sb.AppendLine("and Ada rejects is an input the rest of the web would refuse. If code validates with");
            sb.AppendLine("one parser and fetches with another, that gap is exploitable.");
            sb.AppendLine();
        }

        sb.AppendLine("## Different serialisation");
        sb.AppendLine();

        if (diffs.Count == 0)
        {
            sb.AppendLine("None.");
            return sb.ToString();
        }

        sb.AppendLine("| Input | Ada.Url | System.Uri |");
        sb.AppendLine("| --- | --- | --- |");
        foreach (Divergence d in diffs.OrderBy(d => d.Input, StringComparer.Ordinal))
        {
            sb.Append("| ").Append(Cell(d.Input))
              .Append(" | ").Append(Cell(d.Ada))
              .Append(" | ").Append(Cell(d.Uri))
              .AppendLine(" |");
        }

        return sb.ToString();
    }

    private static string Row(string label, int value)
        => string.Create(CultureInfo.InvariantCulture, $"| {label} | {value} |");

    /// <summary>Makes a corpus value safe to put in a markdown table cell.</summary>
    private static string Cell(string value)
    {
        // The corpus deliberately contains tabs, newlines and pipes, all of which would break the
        // table silently and make the report look shorter than it is.
        string escaped = value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);

        return escaped.Length == 0 ? "*(empty)*" : $"`{escaped}`";
    }

    private sealed record Divergence(string Input, string Ada, string Uri);
}
