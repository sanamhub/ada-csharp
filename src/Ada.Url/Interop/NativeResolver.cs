using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ada.Url.Interop;

/// <summary>
/// Finds the native library when the ordinary loader cannot.
/// </summary>
/// <remarks>
/// For a normal project the NuGet RID graph copies runtimes/{rid}/native next to the app and the
/// platform loader finds it with no help. This resolver covers the cases where that does not
/// happen: single file bundles, custom probe paths, and running straight out of an artifacts
/// directory during development.
/// </remarks>
internal static class NativeResolver
{
    private static int _installed;

    /// <summary>
    /// Installs the resolver before any P/Invoke can run.
    /// </summary>
    /// <remarks>
    /// A module initializer is the documented way to register a <see cref="DllImportResolver"/>,
    /// because it has to be in place before the first native call. The alternative, a static
    /// constructor on the class holding the P/Invokes, would cost that class its
    /// beforefieldinit flag and the inlining that depends on it.
    /// </remarks>
#pragma warning disable CA2255 // ModuleInitializer in a library: see the remarks above.
    [ModuleInitializer]
#pragma warning restore CA2255
    internal static void Install()
    {
        // A module initializer can run more than once across load contexts.
        if (Interlocked.Exchange(ref _installed, 1) != 0)
        {
            return;
        }

        NativeLibrary.SetDllImportResolver(typeof(NativeResolver).Assembly, Resolve);
    }

    private static nint Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, AdaNative.LibraryName, StringComparison.Ordinal))
        {
            return nint.Zero;
        }

        // Let the default loader try first. It succeeds for every normal deployment.
        if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out nint handle))
        {
            return handle;
        }

        string fileName = GetPlatformFileName();
        foreach (string candidate in EnumerateCandidates(fileName))
        {
            if (File.Exists(candidate) && NativeLibrary.TryLoad(candidate, out handle))
            {
                return handle;
            }
        }

        // Returning zero lets the runtime raise its own DllNotFoundException, which names the
        // library and the paths it tried. A custom exception here would say less.
        return nint.Zero;
    }

    private static IEnumerable<string> EnumerateCandidates(string fileName)
    {
        // AppContext.BaseDirectory rather than Assembly.Location, which is empty inside a single
        // file bundle. That bundle is one of the cases this resolver exists to handle.
        string baseDir = AppContext.BaseDirectory;
        string rid = RuntimeInformation.RuntimeIdentifier;

        yield return Path.Combine(baseDir, fileName);
        yield return Path.Combine(baseDir, "runtimes", rid, "native", fileName);

        // Development layout: the artifacts directory that native.yml writes into.
        yield return Path.Combine(baseDir, "artifacts", "native", rid, fileName);
    }

    private static string GetPlatformFileName()
    {
        if (OperatingSystem.IsWindows())
        {
            return $"{AdaNative.LibraryName}.dll";
        }

        return OperatingSystem.IsMacOS()
            ? $"lib{AdaNative.LibraryName}.dylib"
            : $"lib{AdaNative.LibraryName}.so";
    }
}
