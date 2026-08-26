using System.Text;
using System.Text.Json;

namespace Ada.Url.Tests.Conformance;

/// <summary>
/// One case from the web-platform-tests URL corpus.
/// </summary>
/// <param name="Index">Position in the file, so a failure can be found again.</param>
/// <param name="Input">The URL to parse.</param>
/// <param name="Base">The base URL, or null when there is none.</param>
/// <param name="ShouldFail">True when the standard says this input must not parse.</param>
/// <param name="Expected">Expected component values, empty when <paramref name="ShouldFail"/>.</param>
public sealed record UrlCase(
    int Index,
    string Input,
    string? Base,
    bool ShouldFail,
    IReadOnlyDictionary<string, string> Expected)
{
    /// <summary>A short label that identifies the case in test output.</summary>
    public string Describe()
    {
        string label = Base is null ? Input : $"{Input}  (base {Base})";
        label = label.Replace('\n', '␤').Replace('\r', '␍').Replace('\t', '␉');
        return label.Length <= 90 ? label : label[..90] + "...";
    }
}

/// <summary>
/// One case from the setters corpus.
/// </summary>
/// <param name="Index">Position within its setter's array.</param>
/// <param name="Setter">Which component is being assigned.</param>
/// <param name="Href">The URL to start from.</param>
/// <param name="NewValue">The value to assign.</param>
/// <param name="Expected">Component values expected afterwards.</param>
/// <param name="Comment">Upstream's note about the case, when it has one.</param>
public sealed record SetterCase(
    int Index,
    string Setter,
    string Href,
    string NewValue,
    IReadOnlyDictionary<string, string> Expected,
    string? Comment)
{
    /// <summary>A short label that identifies the case in test output.</summary>
    public string Describe()
    {
        string label = $"{Setter} = \"{NewValue}\" on {Href}";
        return label.Length <= 90 ? label : label[..90] + "...";
    }
}

/// <summary>
/// Loads the vendored web-platform-tests corpus.
/// </summary>
/// <remarks>
/// The files are pinned at a known commit and copied to the output directory. They are never
/// fetched at test time, because a test that downloads its own input fails when the network does,
/// and a corpus that changes silently turns a regression into a mystery. See
/// vectors/PROVENANCE.md.
/// </remarks>
public static class WhatwgCorpus
{
    private static readonly string[] ComponentKeys =
    [
        "href", "protocol", "username", "password", "host", "hostname", "port",
        "pathname", "search", "hash",
    ];

    private static readonly Lazy<IReadOnlyList<UrlCase>> LazyParsing = new(LoadParsing);
    private static readonly Lazy<IReadOnlyList<SetterCase>> LazySetters = new(LoadSetters);

    /// <summary>Every parse case in urltestdata.json.</summary>
    public static IReadOnlyList<UrlCase> Parsing => LazyParsing.Value;

    /// <summary>Every setter case in setters_tests.json.</summary>
    public static IReadOnlyList<SetterCase> Setters => LazySetters.Value;

    private static string VectorPath(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "vectors", fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Corpus file '{fileName}' is missing from the test output. It should be copied " +
                "from tests/Ada.Url.Tests/vectors by the project file.", path);
        }

        return path;
    }

    private static List<UrlCase> LoadParsing()
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllBytes(VectorPath("urltestdata.json")));

        var cases = new List<UrlCase>();
        int index = -1;

        foreach (JsonElement element in doc.RootElement.EnumerateArray())
        {
            index++;

            // Plain strings in the array are section comments, not cases.
            if (element.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            // A handful of entries resolve against a non URL base, which this wrapper does not
            // model. Skipping them here rather than letting them fail as if they were bugs.
            if (element.TryGetProperty("relativeTo", out _))
            {
                continue;
            }

            string input = element.GetProperty("input").GetString() ?? string.Empty;

            string? baseUrl = null;
            if (element.TryGetProperty("base", out JsonElement baseElement)
                && baseElement.ValueKind == JsonValueKind.String)
            {
                baseUrl = baseElement.GetString();
            }

            bool shouldFail = element.TryGetProperty("failure", out JsonElement failure)
                              && failure.ValueKind == JsonValueKind.True;

            var expected = new Dictionary<string, string>(StringComparer.Ordinal);
            if (!shouldFail)
            {
                foreach (string key in ComponentKeys)
                {
                    if (element.TryGetProperty(key, out JsonElement value)
                        && value.ValueKind == JsonValueKind.String)
                    {
                        expected[key] = value.GetString() ?? string.Empty;
                    }
                }
            }

            cases.Add(new UrlCase(index, input, baseUrl, shouldFail, expected));
        }

        return cases;
    }

    private static List<SetterCase> LoadSetters()
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllBytes(VectorPath("setters_tests.json")));

        var cases = new List<SetterCase>();

        foreach (JsonProperty setter in doc.RootElement.EnumerateObject())
        {
            // The top level "comment" key is documentation, not a setter.
            if (setter.Value.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            int index = -1;
            foreach (JsonElement element in setter.Value.EnumerateArray())
            {
                index++;

                var expected = new Dictionary<string, string>(StringComparer.Ordinal);
                if (element.TryGetProperty("expected", out JsonElement expectedElement))
                {
                    foreach (JsonProperty property in expectedElement.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.String)
                        {
                            expected[property.Name] = property.Value.GetString() ?? string.Empty;
                        }
                    }
                }

                cases.Add(new SetterCase(
                    index,
                    setter.Name,
                    element.GetProperty("href").GetString() ?? string.Empty,
                    element.GetProperty("new_value").GetString() ?? string.Empty,
                    expected,
                    element.TryGetProperty("comment", out JsonElement comment) ? comment.GetString() : null));
            }
        }

        return cases;
    }

    /// <summary>Reads one component off a parsed URL, as UTF-8 bytes.</summary>
    /// <param name="url">The parsed URL.</param>
    /// <param name="component">One of the WHATWG component names.</param>
    /// <returns>The component value.</returns>
    public static string ReadComponent(in AdaUrl url, string component) => component switch
    {
        "href" => Utf8(url.Href),
        "protocol" => Utf8(url.Protocol),
        "username" => Utf8(url.Username),
        "password" => Utf8(url.Password),
        "host" => Utf8(url.Host),
        "hostname" => Utf8(url.Hostname),
        "port" => Utf8(url.Port),
        "pathname" => Utf8(url.Pathname),
        "search" => Utf8(url.Search),
        "hash" => Utf8(url.Hash),
        _ => throw new ArgumentOutOfRangeException(nameof(component), component, "Unknown component."),
    };

    private static string Utf8(ReadOnlySpan<byte> value) => Encoding.UTF8.GetString(value);
}
