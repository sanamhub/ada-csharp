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
        // Encoding.UTF8 emits a byte order mark, which renders as a stray character before the
        // first heading. UTF8Encoding(false) does not.
        File.WriteAllText(
            path,
            Render(considered, agreed, adaOnly, uriOnly, bothParsed),
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

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
            // Name the row rather than number it. An earlier version said "the third row" while
            // describing the fourth.
            sb.AppendLine(CultureInfo.InvariantCulture, $"**Rejected by Ada, accepted by System.Uri** is the row that matters for security, and it");
            sb.AppendLine(CultureInfo.InvariantCulture, $"holds {uriOnly} of the {considered} cases. Each one is an input `System.Uri` accepts that");
            sb.AppendLine("browsers, Node, Go and Python all refuse. Code that validates a URL with one parser and");
            sb.AppendLine("then fetches it with another has an exploitable gap exactly there.");
            sb.AppendLine();
            sb.AppendLine(CultureInfo.InvariantCulture, $"The other direction, {adaOnly} inputs Ada accepts and `System.Uri` rejects, is a");
            sb.AppendLine("compatibility question rather than a security one. Those are URLs the rest of the web");
            sb.AppendLine("handles and .NET currently does not.");
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
    /// <remarks>
    /// Emits an HTML code element rather than a backtick span. The corpus contains backticks, and
    /// a backtick inside a backtick span closes it early and shreds the rest of the row. It also
    /// contains backslashes, which are already literal inside a code span, so escaping them was
    /// making the report show data the corpus does not contain.
    /// </remarks>
    internal static string Cell(string value)
    {
        if (value.Length == 0)
        {
            return "*(empty)*";
        }

        var sb = new StringBuilder(value.Length + 16);
        sb.Append("<code>");

        foreach (char c in value)
        {
            switch (c)
            {
                case '&': sb.Append("&amp;"); break;
                case '<': sb.Append("&lt;"); break;
                case '>': sb.Append("&gt;"); break;
                case '|': sb.Append("&#124;"); break;   // would end the table cell
                case '`': sb.Append("&#96;"); break;
                case '\r': sb.Append("\\r"); break;
                case '\n': sb.Append("\\n"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    // The corpus includes raw C0 controls. Written through, they would corrupt
                    // the file rather than document anything.
                    if (char.IsControl(c))
                    {
                        sb.Append(CultureInfo.InvariantCulture, $"\\u{(int)c:x4}");
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    break;
            }
        }

        sb.Append("</code>");
        return sb.ToString();
    }

    private sealed record Divergence(string Input, string Ada, string Uri);
}
