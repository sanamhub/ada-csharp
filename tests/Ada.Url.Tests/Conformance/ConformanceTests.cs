using System.Text;
using Xunit;

namespace Ada.Url.Tests.Conformance;

/// <summary>
/// Runs the vendored web-platform-tests URL corpus.
/// </summary>
/// <remarks>
/// This suite is the real specification for this library. Speed is a feature, but agreeing with
/// the standard is the reason the wrapper exists at all, so a regression here matters more than
/// a regression anywhere else.
/// </remarks>
[Trait("Category", "Conformance")]
public class ConformanceTests
{
    public static TheoryData<int, string> ParseCases()
    {
        var data = new TheoryData<int, string>();
        foreach (UrlCase c in WhatwgCorpus.Parsing)
        {
            // The index is what the test looks the case up by. The description is carried only
            // so the test name identifies the input rather than a number.
            data.Add(c.Index, c.Describe());
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(ParseCases))]
    public void Parses_AccordingToTheStandard(int index, string description)
    {
        _ = description;

        UrlCase test = WhatwgCorpus.Parsing.Single(c => c.Index == index);

        byte[] input = Encoding.UTF8.GetBytes(test.Input);
        byte[]? baseUrl = test.Base is null ? null : Encoding.UTF8.GetBytes(test.Base);

        AdaUrl url;
        bool parsed = baseUrl is null
            ? AdaUrl.TryParse(input, out url)
            : AdaUrl.TryParse(input, baseUrl, out url);

        using (url)
        {
            if (test.ShouldFail)
            {
                Assert.False(parsed, $"Expected a parse failure for: {test.Describe()}");
                return;
            }

            Assert.True(parsed, $"Expected a successful parse for: {test.Describe()}");

            foreach ((string component, string expected) in test.Expected)
            {
                string actual = WhatwgCorpus.ReadComponent(url, component);
                Assert.True(
                    expected == actual,
                    $"{component}: expected \"{expected}\" but got \"{actual}\"\n  input: {test.Input}\n  base:  {test.Base ?? "(none)"}");
            }
        }
    }

    public static TheoryData<string, int, string> SetterCases()
    {
        var data = new TheoryData<string, int, string>();
        foreach (SetterCase c in WhatwgCorpus.Setters)
        {
            data.Add(c.Setter, c.Index, c.Describe());
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(SetterCases))]
    public void Setters_BehaveAccordingToTheStandard(string setter, int index, string description)
    {
        _ = description;

        SetterCase test = WhatwgCorpus.Setters.Single(c => c.Setter == setter && c.Index == index);

        byte[] href = Encoding.UTF8.GetBytes(test.Href);
        byte[] value = Encoding.UTF8.GetBytes(test.NewValue);

        Assert.True(AdaUrl.TryParse(href, out AdaUrl url), $"Setup URL did not parse: {test.Href}");

        using (url)
        {
            // The standard says a rejected assignment leaves the URL unchanged rather than
            // raising, so the return value is not asserted. What matters is the state after.
            switch (setter)
            {
                case "protocol": url.TrySetProtocol(value); break;
                case "username": url.TrySetUsername(value); break;
                case "password": url.TrySetPassword(value); break;
                case "host": url.TrySetHost(value); break;
                case "hostname": url.TrySetHostname(value); break;
                case "port": url.TrySetPort(value); break;
                case "pathname": url.TrySetPathname(value); break;
                case "href": url.TrySetHref(value); break;
                case "search": url.SetSearch(value); break;
                case "hash": url.SetHash(value); break;
                default: throw new InvalidOperationException($"Unhandled setter '{setter}'.");
            }

            foreach ((string component, string expected) in test.Expected)
            {
                // searchParams appears in a few expectations and needs the params wrapper,
                // which lands with AdaSearchParams.
                if (component == "searchParams")
                {
                    continue;
                }

                string actual = WhatwgCorpus.ReadComponent(url, component);
                Assert.True(
                    expected == actual,
                    $"after setting {setter} to \"{test.NewValue}\":\n" +
                    $"  {component}: expected \"{expected}\" but got \"{actual}\"\n" +
                    $"  starting href: {test.Href}\n" +
                    $"  upstream note: {test.Comment ?? "(none)"}");
            }
        }
    }

    [Fact]
    public void Corpus_IsLoadedAndLooksRight()
    {
        // Guards against the corpus silently failing to copy to the output directory, which
        // would otherwise show up as a suite that passes because it ran nothing.
        Assert.True(WhatwgCorpus.Parsing.Count > 800, $"Only {WhatwgCorpus.Parsing.Count} parse cases loaded.");
        Assert.True(WhatwgCorpus.Setters.Count > 100, $"Only {WhatwgCorpus.Setters.Count} setter cases loaded.");
        Assert.Contains(WhatwgCorpus.Parsing, c => c.ShouldFail);
        Assert.Contains(WhatwgCorpus.Parsing, c => !c.ShouldFail);
    }
}
