using System.Runtime.InteropServices;

// Restrict native probing to the assembly directory and the OS safe directories. Without this,
// an unqualified load can pick up a planted library from the current working directory.
// NativeResolver still loads by absolute path for single file bundles and development layouts.
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.AssemblyDirectory | DllImportSearchPath.SafeDirectories)]
