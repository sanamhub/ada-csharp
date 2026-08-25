using Ada.Url;
using Xunit;

namespace Ada.Url.Tests;

/// <summary>
/// P0 placeholder so CI has something real to gate on. The WHATWG conformance suite replaces
/// this in P3. See ADA_WRAPPER_PLAN.md section 4.2.
/// </summary>
public class SmokeTests
{
    [Fact]
    public void PinnedUpstreamVersion_IsTheTagWeBuild()
    {
        // Has to stay in step with AdaUrlUpstreamTag in Directory.Build.props and with the
        // --ada-tag default in .github/workflows/native.yml.
        Assert.Equal("4.0.0", AdaUrlInfo.UpstreamAdaVersion);
    }
}
