using Ada.Url;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// Checks that do not need the native library loaded.
/// </summary>
public class SmokeTests
{
    [Fact]
    public void PinnedVersion_MatchesTheTagWeBuild()
    {
        // Has to stay in step with AdaUrlUpstreamTag in Directory.Build.props and with the
        // --ada-tag default in .github/workflows/native.yml.
        Assert.Equal("4.0.0", AdaLibrary.PinnedVersion);
    }
}
