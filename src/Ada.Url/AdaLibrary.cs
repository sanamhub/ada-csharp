using System.Text;
using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// Information about, and process wide settings for, the loaded native library.
/// </summary>
public static class AdaLibrary
{
    /// <summary>The upstream ada-url/ada release this library is built and tested against.</summary>
    public const string PinnedVersion = "4.0.0";

    /// <summary>The version string reported by the native library actually loaded.</summary>
    /// <remarks>
    /// Compare this against <see cref="PinnedVersion"/> when diagnosing a deployment that picked
    /// up the wrong binary.
    /// </remarks>
    public static unsafe string NativeVersion
    {
        get
        {
            byte* p = AdaNative.GetVersion();
            if (p is null)
            {
                return string.Empty;
            }

            int length = 0;
            while (p[length] != 0)
            {
                length++;
            }

            return Encoding.UTF8.GetString(p, length);
        }
    }

    /// <summary>The native library version as major, minor, and revision.</summary>
    /// <returns>The three version numbers.</returns>
    public static (int Major, int Minor, int Revision) GetNativeVersionComponents()
    {
        AdaVersionComponents v = AdaNative.GetVersionComponents();
        return (v.Major, v.Minor, v.Revision);
    }

    /// <summary>
    /// The largest input the parser accepts, in bytes.
    /// </summary>
    /// <remarks>
    /// This is process wide state inside the native library. It affects every consumer in the
    /// process, including other libraries that use Ada, so set it once at startup and never per
    /// request. It is the main control against denial of service through very large inputs.
    /// </remarks>
    public static uint MaxInputLength
    {
        get => AdaNative.GetMaxInputLength();
        set => AdaNative.SetMaxInputLength(value);
    }
}
