using Xunit;

namespace Ada.Url.Tests.Conformance;

/// <summary>
/// The difference report is a document people read, so its escaping is worth testing.
/// </summary>
/// <remarks>
/// The first version used a backtick code span. The corpus contains backticks, which closed the
/// span early and shredded the rest of the row, and it escaped backslashes, which are already
/// literal inside a code span, so the report showed data the corpus does not contain. Both went
/// unnoticed because nothing checked the output.
/// </remarks>
public class ReportEscapingTests
{
    [Fact]
    public void Backtick_DoesNotBreakOutOfTheCell()
    {
        // The exact shape that broke row two of the real report.
        string cell = SystemUriDifferenceReport.Cell("about:blank#^_`az");

        Assert.DoesNotContain("`", cell, StringComparison.Ordinal);
        Assert.Contains("&#96;", cell, StringComparison.Ordinal);
    }

    [Fact]
    public void Pipe_IsEncodedSoItCannotEndTheCell()
    {
        string cell = SystemUriDifferenceReport.Cell("a|b");

        Assert.DoesNotContain("|", cell, StringComparison.Ordinal);
        Assert.Contains("&#124;", cell, StringComparison.Ordinal);
    }

    [Fact]
    public void Backslash_IsPreservedRatherThanDoubled()
    {
        // Inside a code span a backslash is already literal. Doubling it made the report claim
        // the corpus contained two where it contained one.
        string cell = SystemUriDifferenceReport.Cell(@"[\]");

        Assert.Contains(@"[\]", cell, StringComparison.Ordinal);
        Assert.DoesNotContain(@"\\", cell, StringComparison.Ordinal);
    }

    [Fact]
    public void AngleBracketsAndAmpersand_AreHtmlEncoded()
    {
        string cell = SystemUriDifferenceReport.Cell("a<b>c&d");

        Assert.Contains("&lt;", cell, StringComparison.Ordinal);
        Assert.Contains("&gt;", cell, StringComparison.Ordinal);
        Assert.Contains("&amp;", cell, StringComparison.Ordinal);
    }

    [Fact]
    public void ControlCharacters_AreMadeVisible()
    {
        string cell = SystemUriDifferenceReport.Cell("a\tb\nc\rd\u0001e");

        // Verbatim strings. Without the @, C# reads "\t" as a tab and the
        // assertion checks for the wrong thing entirely.
        Assert.Contains(@"\t", cell, StringComparison.Ordinal);
        Assert.Contains(@"\n", cell, StringComparison.Ordinal);
        Assert.Contains(@"\r", cell, StringComparison.Ordinal);
        Assert.Contains(@"\u0001", cell, StringComparison.Ordinal);

        // A raw control character written through would corrupt the file rather than document it.
        Assert.DoesNotContain('\t', cell);
        Assert.DoesNotContain('\n', cell);
        Assert.DoesNotContain('\u0001', cell);
    }

    [Fact]
    public void EmptyValue_IsLabelledRatherThanBlank()
    {
        Assert.Equal("*(empty)*", SystemUriDifferenceReport.Cell(string.Empty));
    }

    [Fact]
    public void OrdinaryValue_IsWrappedInACodeElement()
    {
        Assert.Equal("<code>https://example.com/</code>", SystemUriDifferenceReport.Cell("https://example.com/"));
    }
}
