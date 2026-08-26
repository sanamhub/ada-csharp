# Ada.Url engineering plan

Design and build plan for a .NET wrapper around the [Ada](https://github.com/ada-url/ada) URL
parser. No implementation code ships with this document. Signatures, CMake calls, YAML, and
`.csproj` fragments here are blueprints.

| Field | Value |
| --- | --- |
| Status | Approved. P0, P1, and P2 done. P3 next. |
| Upstream | `ada-url/ada` v4.0.0, pinned by tag |
| ABI reference | `include/ada_c.h` at v4.0.0, captured in section 3.1 |
| Target framework | `net10.0` only. No conditional compilation. See ADR-0001. |
| Distribution | Public package on nuget.org, MIT |
| Reference bindings | [goada](https://github.com/ada-url/goada), [ada-python](https://github.com/ada-url/ada-python) |
| Standards | ADRs in `/docs/adr` and immutable once accepted. OWASP and ASVS for the native build. 80% line coverage target. SemVer with a changelog. Green CI and one approval to merge, two for interop or native changes. |

## Contents

1. [Design stance](#0-design-stance)
2. [Target framework](#1-target-framework)
3. [Native build](#2-native-build)
4. [P/Invoke architecture](#3-pinvoke-architecture)
5. [Test strategy](#4-test-strategy)
6. [Benchmarks](#5-benchmarks)
7. [CI workflows](#6-ci-workflows)
8. [Repository and roadmap](#7-repository-and-roadmap)
9. [Open questions](#8-open-questions)
10. [Notes for the implementer](#9-notes-for-the-implementer)

---

## 0. Design stance

This is a wrapper, not a framework. Ada does the hard part. Anything we add is overhead, in
latency, in binary size, or in maintenance, so the plan is judged by how little it adds.

Five facts drive the rest of the document.

**Ada's C ABI is byte oriented UTF-8.** Every entry point takes `const char*` plus `size_t`. So
the API is UTF-8 first. A `ReadOnlySpan<byte>` costs no conversion. A `string` costs a transcode
that no amount of cleverness removes.

**Zero allocation only holds for span in, span out.** A `System.String` result allocates by
construction. Every published number is tiered (section 5.1) so the zero byte figure is never
quoted out of context.

**Borrowed versus owned strings is the top correctness hazard.** `ada_string` and
`ada_owned_string` are the same struct. Four functions return memory that leaks if unfreed, and
every getter returns a pointer that dangles after any mutation. Section 3.2 has the table.

**Struct return by value is the top silent hazard.** `ada_string` is 16 bytes. Win64 returns it
through a hidden pointer, SysV x86-64 in `RAX:RDX`, AArch64 in `X0:X1`. .NET handles blittable
structs correctly on all three, but a mismatch here is memory corruption, not a bug report.
Section 4.5 gates it per platform.

**Thin means thin.** Bind the ABI, manage lifetime, get out of the way. No abstraction layers,
no dependency injection, no interfaces over a P/Invoke call.

### 0.1 What this plan does not do

Cut after a minimality review. Each was defensible. None was worth its cost.

| Cut | Reason |
| --- | --- |
| `netstandard2.0` and `net8.0` as extra targets | The floor TFM was the largest complexity driver in the first draft: a second interop file, a transcode fallback, an MSBuild file to rescue native loading on .NET Framework, a `System.Memory` dependency, and extra CI legs. All for a consumer that does not exist. Section 1 keeps the analysis as the contingency. |
| Four project Clean Architecture split | Ceremony for five public types. Flat layout instead, see ADR-0005. |
| Architecture test enforcing layer direction | There are two layers. A test to prove it is theatre. |
| Custom Roslyn analyzer for a missing `using` | An analyzer project plus a test project to catch one mistake. Handled instead by making the handle free API the primary surface, so most callers never own a handle. |
| Span wrapper with a generation counter | It would appear in the signature of every getter, taxing the path that has to stay free. Replaced by contract, tests, and ASAN. |
| Always on `EventCounter` handle tracking | This was a real defect in the first draft. An `Interlocked.Increment` per parse is measurable against a 50 ns operation. Now `ADA_DIAGNOSTICS` builds only. |
| A separate AVX2 pinned package | A hidden CPU floor for a few percent on a workload nobody has measured. A second package is also a second thing to build, sign, version, and support. |
| Per RID runtime packages via `runtime.json` | One package. Revisit if the native grows. |
| Nightly fuzzing lane | Upstream fuzzes the native side. Our surface is transcode and length arithmetic, covered by property based tests in the normal suite. |
| valgrind, Application Verifier, macOS `leaks`, RSS slope tracking | Four extra leak mechanisms on top of ASAN. Our code is platform independent, so Linux ASAN plus handle counters catch almost all of it. |
| Component slicing fast path in v1 | Ten suppressed transition getters cost about 2 ns each against a 50 ns parse. Slicing adds real sentinel and offset risk for a gain that has to be measured first. See section 3.7. |
| `/GR-` in the native build | Needs proof that upstream does not use RTTI, for a small size win. |

---

## 1. Target framework

### 1.1 Decision

```xml
<TargetFramework>net10.0</TargetFramework>
```

One target. No `#if` anywhere in the repository, checked by CI.

The brief asked whether multi targeting down to `netstandard2.0` was possible and what it would
cost. It is possible, and section 1.4 gives the working strategy. It costs more than it is worth
here, so we do not do it. Sections 1.2 and 1.3 are the analysis behind that call. They stay
because they are a required deliverable and because they are the contingency if a .NET Framework
consumer ever appears.

`net10.0` gives us `[LibraryImport]`, `[SuppressGCTransition]`, `Utf8.FromUtf16`,
`[SkipLocalsInit]`, real `ref` fields in `Span<T>`, and a clean NativeAOT story. Each of those is
load bearing for the overhead target.

Dropping the floor removed the second interop file, `LegacyTranscode`, the
`build/net472/Ada.Url.targets` rescue, the `System.Memory` reference, the `net472` CI legs, the
`net48` benchmark job, and the risk of a `#if` quietly changing behaviour on one target.

### 1.2 What the floor target would have cost

Kept as the rejection rationale.

Legend: yes, available and used. Partial, available through the `System.Memory` polyfill with
reduced JIT support. No, unavailable.

| Capability | net10.0 | netstandard2.0 | Effect when missing |
| --- | :-: | :-: | --- |
| `[LibraryImport]` source generation | yes | no | Falls back to `[DllImport]` and a runtime IL stub per call site |
| Blittable signatures with no stub | yes | partial | Reachable on the floor because we use only `byte*`, `nuint`, `nint`, and blittable structs. This recovers most of the gap. |
| `ReadOnlySpan<T>` as a runtime primitive | yes | partial | The polyfill has no `ref` field and an extra indirection, so it is slower even when nothing allocates |
| `Utf8.FromUtf16` with `OperationStatus` | yes | no | Falls back to `Encoding.UTF8.GetBytes` over pinned pointers, with no status result, so invalid surrogates need a pre scan |
| `[SkipLocalsInit]` | yes | no | Every `stackalloc` scratch buffer is zeroed on every call |
| `[SuppressGCTransition]` | yes | no | `ada_has_port`, a native field read, pays a full GC mode transition |
| `NativeLibrary.SetDllImportResolver` | yes | no | No programmatic native probing, which is what forces the MSBuild rescue file |
| `GC.GetAllocatedBytesForCurrentThread()` | yes | no | The zero byte assertion cannot run there, so no allocation claim can be made |
| NativeAOT and trimming | yes | no | The package cannot advertise AOT support unconditionally |

### 1.3 Why the floor was rejected

1. **No source generated marshalling.** With blittable signatures the `DllImport` stub is close
   to a direct call, so the gap is smaller than folklore. It is still opaque, and nothing catches
   a signature that drifts non blittable.
2. **Non blittability fails silently there.** `LibraryImport` makes it a compile error.
   `DllImport` makes it a per call heap allocation that compiles fine.
3. **Polyfill `Span<T>` is slower** on every span operation, allocation or not.
4. **Scratch buffers are zeroed** on every transcode.
5. **Full GC transitions on trivial calls,** which dominates predicate heavy code such as an
   allow list check in a loop.
6. **No programmatic native resolution.** This is the classic native NuGet failure, and it has to
   be fixed in the package rather than documented away.

Items 1 and 2 are mostly recoverable. Items 4, 5, and 6 are not. The sum is a slower build with
non zero allocations, no way to test allocation, and a permanent second code path, to serve
nobody.

### 1.4 The conditional compilation strategy we are not using

With one target there is nothing to condition on. This section records the strategy so the
decision does not have to be worked out again.

Governing rule: `#if` may change the mechanism, never the observable behaviour. Enforced by
running the whole conformance suite against every target with identical expectations. A test that
needs `#if` on its assertions is a design defect, not a platform difference. The boundary would be
`#if NET`, never a version ladder.

```
src/Ada.Url/                        (contingency layout)
  Interop/
    AdaNative.cs          blittable structs and constants, no #if
    AdaNative.Modern.cs   #if NET   -> [LibraryImport] partials
    AdaNative.Legacy.cs   #else     -> [DllImport] externs
    Transcode.cs          the only behaviour neutral #if
  *.cs                    public API, no #if on any configuration
```

The shipped build is the modern half, unconditioned:

```csharp
internal static unsafe partial class AdaNative
{
    [LibraryImport(LibraryName, EntryPoint = "ada_parse")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint Parse(byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_is_valid")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]                  // leaf: cannot block, allocate, or call back
    internal static partial byte IsValid(nint url);
}
```

Every detail there is load bearing. `byte` return instead of `bool`, because a `bool` return
forces marshalling configuration and a stub, and C `_Bool` is one byte. `ExactSpelling = true`
skips the `W` and `A` probe walk. `SetLastError = false` skips a `GetLastError` capture per call.
Explicit `Cdecl`, because relying on the platform default is how ARM64 diverges.

Transcode ships without a conditional:

```csharp
internal static OperationStatus Utf16ToUtf8(ReadOnlySpan<char> src, Span<byte> dst, out int written)
    => Utf8.FromUtf16(src, dst, out _, out written, replaceInvalidSequences: false);
```

Shared build settings live in `Directory.Build.props`. `TreatWarningsAsErrors` plus
`AnalysisLevel=latest-all` makes lint a compiler gate rather than a review convention, which is
what a lint gate is for.

### 1.5 Acceptance criteria

| ID | Criterion |
| --- | --- |
| AC-1.1 | Smoke test passes on Windows, Linux, and macOS, on x64 and arm64 |
| AC-1.2 | No `#if` anywhere in `src` or `tests`, checked by CI grep |
| AC-1.3 | A test asserts `!RuntimeHelpers.IsReferenceOrContainsReferences<T>()` for every interop type |
| AC-1.4 | NativeAOT publish succeeds with no trim or AOT warnings, and the smoke test passes against the AOT binary |
| AC-1.5 | A clean project installing the published package from nuget.org parses a URL on every shipped RID |

---

## 2. Native build

### 2.1 Upstream options

| Option | Upstream default | Ours | Why |
| --- | :-: | :-: | --- |
| `BUILD_SHARED_LIBS` | OFF | **ON** | We ship a shared library, so this has to be explicit |
| `ADA_TESTING`, `ADA_BENCHMARKS`, `ADA_TOOLS` | OFF | OFF | Release artifacts only |
| `ADA_USE_SIMDUTF` | OFF | **OFF** | Tried and rejected. With `BUILD_SHARED_LIBS=ON` it builds simdutf as a second shared library, which breaks the Windows build and adds a runtime dependency elsewhere. See ADR-0003. |
| `ADA_USE_UNSAFE_STD_REGEX_PROVIDER` | OFF | OFF | Upstream test switch, never in a shipped artifact |
| `CMAKE_INTERPROCEDURAL_OPTIMIZATION` | not set | **ON** | Portable LTO, plus explicit per compiler flags |
| `WINDOWS_EXPORT_ALL_SYMBOLS` | YES | inherited | No `__declspec(dllexport)` patching needed on Windows |

Upstream does not set `CMAKE_CXX_STANDARD` in the root `CMakeLists.txt`. It delegates to
`cmake/ada-flags.cmake`, so we set it explicitly rather than assume. The exported target is
`ada` with `SOVERSION 4`, which fixes the Linux filename to `libada.so.4`.

### 2.2 SIMD and the CPU floor

The brief asked for AVX2 and BMI2 flags to keep Ada's SIMD speed. Compiling the whole library
that way keeps it and raises the minimum CPU to Haswell, from 2013. Below that the process dies
with `SIGILL`. Not a fallback, a crash, possibly on a customer VM with a masked feature set.

Ship an `x86-64-v2` baseline. That keeps the floor at 2009 hardware, and Ada keeps its own SIMD
paths regardless of build flags.

The first attempt reached for `ADA_USE_SIMDUTF=ON`, since simdutf dispatches its kernel at
runtime and looked like SIMD speed with no static floor. The first CI run killed that idea.
`BUILD_SHARED_LIBS=ON` propagates into simdutf and builds it as a second shared library. On
Windows `cmake -E __create_def` crashes with `0xC0000005` generating its exports. On Linux and
macOS it works but leaves `libada.so` depending on `libsimdutf.so.25`, which means two binaries
per RID and rpath handling. `BUILD_SHARED_LIBS` is global, so there is no command line way to
link simdutf statically into a shared Ada.

One shared object per RID, no simdutf. Recorded in ADR-0003.

### 2.3 RIDs and artifact names

| RID | File | Runner | Note |
| --- | --- | --- | --- |
| `win-x64` | `ada.dll` | `windows-2022` | PDB goes to a symbol server, not the package |
| `linux-x64` | `libada.so` | `ubuntu-24.04` | NuGet has to contain the real file named `libada.so`. Symlinks are not portable in a nupkg, so copy the resolved binary and rename. |
| `linux-arm64` | `libada.so` | `ubuntu-24.04-arm` | |
| `linux-musl-x64` | `libada.so` | `ubuntu-24.04` with an Alpine container | Required for containers. A glibc `.so` will not load on Alpine. |
| `osx-x64` | `libada.dylib` | `macos-13` | |
| `osx-arm64` | `libada.dylib` | `macos-14` | |

`win-arm64` is deferred, see O-1. Managed code declares one name, `const string LibraryName =
"ada"`, and each loader adds its own prefix and suffix. That is why the Linux file has to be
named exactly `libada.so`.

Two macOS RIDs, not a `lipo` universal binary. The RID graph already picks the right asset, and a
universal binary doubles the size of every deployment for nothing. The `lipo` recipe stays in the
build script for single file bundle consumers.

### 2.4 Build invocations

```bat
:: Windows x64, MSVC 17.x, x86-64-v2 baseline
git clone --depth 1 --branch v4.0.0 https://github.com/ada-url/ada.git ada-src
cmake -S ada-src -B build/win-x64 -G "Visual Studio 17 2022" -A x64 ^
  -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON ^
  -DADA_TESTING=OFF -DADA_BENCHMARKS=OFF -DADA_TOOLS=OFF ^
  -DADA_USE_SIMDUTF=OFF -DCMAKE_INTERPROCEDURAL_OPTIMIZATION=ON ^
  -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON ^
  -DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreadedDLL ^
  -DCMAKE_CXX_FLAGS_RELEASE="/O2 /Ob3 /Oi /GL /Gy /Gw /EHsc /DNDEBUG /Zi /guard:cf" ^
  -DCMAKE_SHARED_LINKER_FLAGS_RELEASE="/LTCG /OPT:REF /OPT:ICF /INCREMENTAL:NO /DEBUG /GUARD:CF /DYNAMICBASE /HIGHENTROPYVA /CETCOMPAT"
cmake --build build/win-x64 --config Release --parallel
```

```bash
# Linux x64 (glibc), Clang
cmake -S ada-src -B build/linux-x64 -G Ninja \
  -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON \
  -DADA_TESTING=OFF -DADA_BENCHMARKS=OFF -DADA_TOOLS=OFF \
  -DADA_USE_SIMDUTF=OFF -DCMAKE_INTERPROCEDURAL_OPTIMIZATION=ON \
  -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON \
  -DCMAKE_C_COMPILER=clang -DCMAKE_CXX_COMPILER=clang++ \
  -DCMAKE_CXX_FLAGS_RELEASE="-O3 -DNDEBUG -march=x86-64-v2 -mtune=generic \
     -flto=thin -fno-plt -fno-semantic-interposition -fvisibility=hidden \
     -ffunction-sections -fdata-sections -fstack-protector-strong \
     -D_FORTIFY_SOURCE=2 -fcf-protection=full" \
  -DCMAKE_SHARED_LINKER_FLAGS_RELEASE="-flto=thin -Wl,--gc-sections -Wl,-O2 \
     -Wl,--as-needed -Wl,-z,relro,-z,now -Wl,-z,noexecstack"
cmake --build build/linux-x64 --parallel
llvm-strip --strip-unneeded build/linux-x64/libada.so.4.0.0   # keep an unstripped copy

# linux-arm64: swap -march=x86-64-v2 -mtune=generic for
#              -march=armv8-a+crc+crypto -mtune=neoverse-n1.
#              NEON is ARMv8 baseline, so simdutf's NEON kernels are always available.
# linux-musl-x64: same, inside an Alpine container with clang and musl-dev.
#              Drop -D_FORTIFY_SOURCE=2 if musl warns. Its coverage differs from glibc.
```

```bash
# macOS, one build per arch, two RIDs
for ARCH in x86_64 arm64; do
  [ "$ARCH" = x86_64 ] && { AF="-march=x86-64-v2"; MIN=10.15; } || { AF="-mcpu=apple-m1"; MIN=11.0; }
  cmake -S ada-src -B "build/osx-$ARCH" -G Ninja \
    -DCMAKE_BUILD_TYPE=Release -DBUILD_SHARED_LIBS=ON \
    -DADA_TESTING=OFF -DADA_BENCHMARKS=OFF -DADA_TOOLS=OFF \
    -DADA_USE_SIMDUTF=OFF -DCMAKE_INTERPROCEDURAL_OPTIMIZATION=ON \
    -DCMAKE_CXX_STANDARD=20 -DCMAKE_CXX_STANDARD_REQUIRED=ON \
    -DCMAKE_OSX_ARCHITECTURES="$ARCH" -DCMAKE_OSX_DEPLOYMENT_TARGET="$MIN" \
    -DCMAKE_CXX_FLAGS_RELEASE="-O3 -DNDEBUG -flto=thin -fvisibility=hidden -fstack-protector-strong $AF" \
    -DCMAKE_SHARED_LINKER_FLAGS_RELEASE="-flto=thin -Wl,-dead_strip"
  cmake --build "build/osx-$ARCH" --parallel
done
codesign --force --timestamp --sign "$MACOS_SIGNING_IDENTITY" build/osx-arm64/libada.dylib
# The identity comes from the CI secret store. Never inline it, in a script or in this document.
```

Flag notes:

- `/GL` and `/LTCG` have to be paired or `/GL` silently degrades.
- `/Ob3` inlines harder than `/O2`, which defaults to `/Ob2`.
- `/guard:cf`, `/DYNAMICBASE`, `/HIGHENTROPYVA`, `/CETCOMPAT`, `-Wl,-z,relro,-z,now`, and
  `-fcf-protection` are required hardening, not optional. A native library without them does not
  pass security review.
- `MultiThreadedDLL` matches what .NET processes already load. A static CRT inside a DLL next to
  .NET is a heap mismatch waiting to happen.
- `-mcpu=apple-m1` is safe only because `osx-arm64` implies Apple Silicon. Do not copy it into
  the `linux-arm64` build, where the hardware is unknown.

`-fvisibility=hidden` has a trap. On ELF and Mach-O it exports only annotated symbols. If upstream
does not annotate its C API, we ship a library that builds and exports nothing. Hard CI gate:

```bash
test "$(nm -gDU build/linux-x64/libada.so | grep -c ' T ada_')" -gt 0
```

If it fails, drop the flag. Do not patch upstream. This is AC-2.2 and it is the silent failure
mode of this section.

### 2.5 NuGet layout

```
Ada.Url.<version>.nupkg
  lib/net10.0/          Ada.Url.dll  Ada.Url.xml
  runtimes/{win-x64,linux-x64,linux-arm64,linux-musl-x64,osx-x64,osx-arm64}/native/
  README.md  LICENSE  THIRD-PARTY-NOTICES.txt  icon.png
```

For a `net10.0` consumer the RID graph resolves `runtimes/{rid}/native/*` and copies it next to
the app, with no configuration.

Shipping one target removes the whole class of native resolution failure a `netstandard2.0` asset
would have introduced. No `build/*/Ada.Url.targets` rescue and no `$(PlatformTarget)` guessing.
Two cases are still handled in code rather than MSBuild, single file bundles and custom probe
paths, through the `DllImportResolver` in section 3.3.

Packing fragment:

```xml
<ItemGroup>
  <!-- One line per RID. CopyToOutputDirectory makes local F5 work with no install step. -->
  <None Include="$(NativeRoot)win-x64\ada.dll" Pack="true" PackagePath="runtimes/win-x64/native/"
        CopyToOutputDirectory="PreserveNewest" Link="runtimes\win-x64\native\ada.dll" />
  <None Include="$(NativeRoot)linux-x64\libada.so" Pack="true" PackagePath="runtimes/linux-x64/native/" />
</ItemGroup>
```

No 32 bit native ships. `win-x86` is not a supported RID, and with no `netstandard2.0` asset
there is no path by which a 32 bit project picks up an x64 DLL and fails at load. The RID graph
simply finds nothing.

One package containing all natives, around 2 to 5 MB, is one artifact to version and sign. See
ADR-0003.

### 2.6 Supply chain

This is a gate, not a nice to have. A hand built DLL dropped into a repository fails every item
below.

- Natives are built only in CI, from the pinned tag, on ephemeral runners, in containers pinned
  by digest where containers are used.
- A SHA-256 manifest lives at `native/CHECKSUMS.txt` and is committed. The pack step verifies
  against it and fails on mismatch, so tampering breaks the build instead of shipping.
- An SBOM in CycloneDX format covers Ada and simdutf, published with every release.
- The Windows DLL is code signed. The macOS dylib is signed and notarised, since unsigned dylibs
  are increasingly blocked. Identities come from the CI secret store only.
- Managed builds are deterministic and SourceLinked.
- Native binaries are never committed. They are CI artifacts consumed by the pack job. Only
  `CHECKSUMS.txt` is in git.

### 2.7 Acceptance criteria

| ID | Criterion |
| --- | --- |
| AC-2.1 | One workflow produces all six natives from tag v4.0.0 with no manual step |
| AC-2.2 | Export gate passes. `ada_parse`, `ada_free`, `ada_get_href`, and `ada_free_owned_string` are present in every artifact. A missing symbol fails the build. |
| AC-2.3 | Baseline artifacts run on an `x86-64-v2` only CPU, checked with `qemu-x86_64 -cpu Nehalem` or a feature masked VM. No `SIGILL`. |
| AC-2.4 | `dumpbin /headers` shows CFG, ASLR, and high entropy VA. `checksec` shows RELRO, NX, and stack protector. |
| AC-2.5 | An Alpine container test proves the musl RID loads and the glibc RID does not silently substitute |
| AC-2.6 | SBOM and checksum manifest published per release, and pack fails on mismatch |
| AC-2.7 | A NativeAOT published app resolves the native library and parses a URL on each shipped RID |

---

## 3. P/Invoke architecture

### 3.1 The ABI at v4.0.0

Opaque handles map to `nint`: `ada_url`, `ada_url_search_params`, `ada_strings`, and the three
search params iterators.

By value structs, which are the risk surface for section 4.5:

```c
typedef struct { const char* data; size_t length; } ada_string;        // BORROWED, never free
typedef struct { const char* data; size_t length; } ada_owned_string;  // OWNED, must free
typedef struct { ada_string key; ada_string value; } ada_string_pair;
typedef struct { uint32_t protocol_end, username_end, host_start, host_end,
                          port, pathname_start, search_start, hash_start; } ada_url_components;
typedef struct { int major, minor, revision; } ada_version_components;

#define ada_url_omitted 0xffffffff   /* uint32_t(-1), component absent */
```

`ada_string` and `ada_owned_string` are structurally identical, and the difference lives only in
the header's prose. Managed code has to encode it in the type system, because a comment will not
survive maintenance. See section 3.5.

Lifetime: `ada_parse`, `ada_parse_with_base`, `ada_can_parse`, `ada_can_parse_with_base`,
`ada_free`, `ada_free_owned_string`, `ada_copy`, `ada_is_valid`.

> Checked against the v4.0.0 header: `ada_parse_and_validate`, which goada calls, does not exist.
> Use `ada_parse` then `ada_is_valid`, and detect a failed parse before any other call.

Borrowed getters, each `ada_string f(ada_url)`: `ada_get_href`, `_username`, `_password`, `_port`,
`_hash`, `_host`, `_hostname`, `_pathname`, `_search`, `_protocol`.

Owned getter: `ada_owned_string ada_get_origin(ada_url)`.

Enums: `uint8_t ada_get_host_type`, `ada_get_scheme_type`.

Setters returning `bool`: `ada_set_href`, `_host`, `_hostname`, `_protocol`, `_username`,
`_password`, `_port`, `_pathname`, all `bool f(ada_url, const char*, size_t)`. Setters returning
`void`: `ada_set_search`, `ada_set_hash`. Clears: `ada_clear_port`, `_hash`, `_search`.

Predicates, all `bool f(ada_url)`: `ada_has_credentials`, `_empty_hostname`, `_hostname`,
`_non_empty_username`, `_non_empty_password`, `_port`, `_password`, `_hash`, `_search`.

Components: `const ada_url_components* ada_get_components(ada_url)`, a pointer to internal state.

IDNA, both owned: `ada_idna_to_unicode`, `ada_idna_to_ascii`.

Search params: `ada_parse_search_params`, `_free_search_params`, `_size`, `_sort`, `_to_string`
(owned), `_append`, `_set`, `_remove`, `_remove_value`, `_has`, `_has_value`, `_get` (borrowed),
`_get_all` returning `ada_strings`, `_reset`, `_get_keys`, `_get_values`, `_get_entries` returning
iterators, plus `ada_free_strings`, `ada_strings_size`, `ada_strings_get`, and per iterator
`*_free`, `*_next`, `*_has_next`. `entries_iter_next` returns `ada_string_pair`.

Global: `ada_set_max_input_length`, `ada_get_max_input_length`, `ada_get_version`,
`ada_get_version_components`.

### 3.2 Borrowed and owned

| Function | Returns | Free? | Invalidated by |
| --- | --- | :-: | --- |
| The ten `ada_get_*` string getters | borrowed | no | any `ada_set_*`, `ada_clear_*`, or `ada_free` |
| `ada_search_params_get` | borrowed | no | any params mutation or free |
| `ada_strings_get`, `*_iter_next` | borrowed | no | parent free or iterator advance |
| `ada_get_components` | borrowed pointer | no | any `ada_set_*`, `ada_clear_*`, or `ada_free` |
| `ada_get_origin` | owned | **yes**, `ada_free_owned_string` | |
| `ada_search_params_to_string` | owned | **yes** | |
| `ada_idna_to_ascii`, `_to_unicode` | owned | **yes** | |
| `ada_parse`, `_with_base`, `ada_copy` | handle | **yes**, `ada_free` | |
| `ada_parse_search_params` | handle | **yes**, `ada_free_search_params` | |
| `ada_search_params_get_all` | `ada_strings` | **yes**, `ada_free_strings` | |
| `*_get_keys`, `_values`, `_entries` | iterator | **yes**, matching `*_free` | |

Four owned string functions leak if mishandled. Every borrowed pointer becomes a use after free
the moment the URL mutates.

### 3.3 Declaration rules

1. **Blittable only.** `byte*`, `nint`, `nuint`, `uint`, `byte`, and explicit layout blittable
   structs. No `string`, no `bool`, no `[MarshalAs]`, no arrays. On `net10.0` a non blittable type
   in a `[LibraryImport]` signature is a compile error, so the rule enforces itself.
2. **`size_t` maps to `nuint`,** never `int` or `uint`. A wrong width corrupts the stack on one
   platform and passes tests on another.
3. **C `bool` maps to `byte`,** converted in managed code.
4. **Always `ExactSpelling = true`, `SetLastError = false`, and explicit `Cdecl`.**
5. **`[SuppressGCTransition]` only on true leaves.** The permitted set is the nine `ada_has_*`
   predicates, `ada_is_valid`, `ada_get_host_type`, `ada_get_scheme_type`,
   `ada_get_max_input_length`, and the ten borrowed getters. Never on `ada_parse`,
   `ada_parse_with_base`, any `ada_set_*`, or the IDNA functions. A suppressed transition delays
   GC for the length of the call, and misplacing it causes pauses that are very hard to trace.
   The permitted set is listed in the file header and checked in CI.
6. **Explicit layout on struct returns.**
   ```csharp
   [StructLayout(LayoutKind.Sequential)]
   internal readonly unsafe struct AdaString { public readonly byte* Data; public readonly nuint Length; }
   ```
7. **One library name,** `const string LibraryName = "ada"`. The loader adds prefix and suffix.
8. **A `DllImportResolver` from a module initializer,** for single file bundles and custom probe
   paths where the plain loader plus RID asset copy is not enough.

### 3.4 Data flow

```
UTF-8 in, the zero allocation path
  ReadOnlySpan<byte> utf8
    fixed (byte* p = utf8)                        pin, no copy, no allocation
      AdaNative.Parse(p, (nuint)utf8.Length)      -> nint handle
        AdaString s = AdaNative.GetHref(handle)   borrowed, no copy
          new ReadOnlySpan<byte>(s.Data, (int)s.Length)   zero copy view
  Managed allocation: 0 bytes.

UTF-16 in, convenience with a documented cost
  byteCount = 3 * utf16.Length                    worst case
  byteCount <= 512 ? stackalloc byte[512]         with SkipLocalsInit, not zeroed
                   : ArrayPool<byte>.Shared.Rent(byteCount)
  Utf8.FromUtf16(utf16, scratch, out _, out written)     OperationStatus, no exceptions
  fixed -> Parse, then return the rented array in a finally
  Managed allocation: 0 bytes in steady state. The transcode CPU cost is real and measured.

string out
  Encoding.UTF8.GetString(nativeSpan)             one allocation, unavoidable
  Benchmarked as its own tier, never quoted as the zero byte number.
```

Three consequences worth stating so they do not get lost.

The `stackalloc` threshold is 512 bytes, about 170 worst case UTF-16 characters, then
`ArrayPool`. Never `new byte[n]` on any path. The constant is tuned by benchmark W4, not guessed.

Surrogates are handled explicitly. `replaceInvalidSequences: false` gives
`OperationStatus.InvalidData`, which becomes `false` from `TryParse`. No
`EncoderFallbackException` escapes the hot path, because exceptions must not be control flow here.

Input length is checked in managed code as well, so behaviour does not depend on who last touched
the process global `ada_set_max_input_length`.

Why not `[LibraryImport(StringMarshalling = Utf8)]`? It is correct, but it allocates and copies
through `Marshal.StringToCoTaskMemUTF8` on every call. We pin managed bytes with `fixed` instead:
no allocation, no copy, and a claim that `[DisassemblyDiagnoser]` can check. For the record, the
naive `[DllImport] static extern nint ada_parse(string, nuint)` is worse. Default marshalling
would transcode as ANSI and mangle non ASCII URLs, which is exactly the IDNA case this library
exists to get right.

### 3.5 Lifetime, three pieces

**Handle free statics, the primary API.** Most real use is validate and normalise in a request
pipeline. Nothing escapes and nothing needs disposing.

```csharp
public static bool CanParse(ReadOnlySpan<byte> utf8, ReadOnlySpan<byte> baseUrl = default);
public static bool TryNormalize(ReadOnlySpan<byte> input, Span<byte> destination, out int written);
public static bool TryGetOrigin (ReadOnlySpan<byte> input, Span<byte> destination, out int written);
public static bool TryGetHostname(ReadOnlySpan<byte> input, Span<byte> destination, out int written);
```

Parse, use, and free happen inside one call, and even the result allocates nothing. This is the
documented default, and it is what replaces the analyzer we cut.

**`AdaUrl`, a stack bound `ref struct`,** for work touching several properties.

```csharp
public ref struct AdaUrl : IDisposable
{
    public static bool TryParse(ReadOnlySpan<byte> utf8, out AdaUrl url);
    public ReadOnlySpan<byte> Href { get; }            // borrowed, zero copy
    public bool TrySetHost(ReadOnlySpan<byte> utf8);   // invalidates prior borrowed spans
    public void Dispose();                             // ada_free
}
```

No object header, no finalizer, no GC tracking. The `ref struct` rules stop the handle reaching a
field, a lambda capture, an async state machine, or the heap, so the compiler enforces the frame
scoped lifetime that goada maintains by hand with `runtime.KeepAlive`. All performance claims are
made here. The residual risk is a forgotten `Dispose`, which is R-3.

**`AdaUrlHandle : SafeHandle`,** for fields, caches, async, and cross thread ownership. One small
allocation and a finalizer. `ReleaseHandle` calls `ada_free`, and `DangerousAddRef` and
`DangerousRelease` bracket borrowed reads so the handle cannot be freed under a live span. This is
the analogue of goada's `runtime.AddCleanup` plus `runtime.KeepAlive`. Reached through
`AdaUrl.ToHandle()`, so the cost is opted into visibly.

Owned strings get a disposable ref struct so the free cannot be skipped:

```csharp
public ref struct AdaOwnedString : IDisposable   // ada_free_owned_string
{
    public ReadOnlySpan<byte> Span { get; }
    public int CopyTo(Span<byte> destination);
    public void Dispose();
}
```

Public APIs prefer `Try...(..., Span<byte> destination, out int written)`, so callers never see
native memory or a lifetime. `AdaOwnedString` exists for callers who want to skip the copy.

### 3.6 Use after free

A span over native memory dangles after any setter. The first draft proposed a wrapper struct
with a generation counter, but that wrapper would appear in the signature of every getter, taxing
the path that has to stay free. Instead:

- **Contract.** Any `ada_set_*` or `ada_clear_*` invalidates every span previously returned from
  that URL. Documented on every getter and setter.
- **Shape.** The handle free statics have no exposure at all, and they are the documented default.
- **For values held across a mutation,** use the `CopyTo(Span<byte>)` accessors. This is the rule,
  not an option.
- **Verification.** Test category 15 asserts the documented behaviour, and Linux ASAN over the
  full conformance suite catches a real use after free in our own code.

This is a deliberate trade, recorded in ADR-0004, and R-2 stays open in the register rather than
being marked mitigated.

### 3.7 Components, measure before building

`ada_get_components` returns eight `uint32` offsets into the already serialised `href`, so one
`href` read plus one components read could replace ten getters by slicing.

Not in v1. Ten suppressed transition getters cost about 2 ns each against a 50 ns parse, so this
is not the bottleneck, and the slicing layer adds real correctness risk. Every field can carry
`ada_url_omitted`, and an unchecked cast to `int` gives -1 and an out of range slice.

v1 ships the direct getters plus the raw struct read:

```csharp
public const uint Omitted = 0xFFFFFFFFu;                       // checked against v4.0.0
public bool TryGetComponents(out AdaUrlComponents components); // ref readonly reinterpret, no copy
```

Benchmark W1 includes a ten getters versus slicing variant. Build the slicing layer only if that
measurement justifies it. Test category 14 covers the sentinel either way.

### 3.8 Threading, global state, security

**Handles are not thread safe.** One per thread, or synchronise externally. Concurrent reads of an
unmutated handle are fine. One concurrent setter makes every outstanding span a use after free.

**`ada_set_max_input_length` is process global.** Set it once from a module initializer and expose
it as a documented global switch, never a per call parameter. It affects every consumer in the
process, including other libraries. Enforce the limit in managed code too, so behaviour does not
depend on call ordering between unrelated components.

**Denial of service, OWASP API4.** The length cap is the control. Ada v4.0.0's release notes cite
fixes for DoS on malformed UTF-8 and tighter input length enforcement, which is a good reason to
pin v4.x rather than v3.x.

**SSRF guidance belongs in the README, OWASP A10.** A URL parser is usually used to decide whether
a URL is safe to fetch. Two rules to publish: compare the parsed `hostname`, never the raw input,
and match allow lists against post IDNA ASCII, because confusable Unicode domains normalise to
different ASCII. Getting this wrong is an exploitable SSRF.

**No secrets.** URLs routinely carry credentials in `username` and `password`. The library must
never log a full `href` at any level, and the README says so. Signing identities live in the CI
secret store only.

### 3.9 Parity with the official bindings

| Reference | Ada.Url | Note |
| --- | --- | --- |
| ada-python `URL` settable properties | `AdaUrl` and `AdaUrlHandle` properties plus `TrySet*` | Python raises on failure. Ours returns `bool`, as the C ABI does, with a throwing `Set*` convenience. |
| ada-python `parse_url()` returning a dict | `AdaUrlComponents` struct plus getters | Not a `Dictionary<string,string>`. That is the dynamic property map made allocation free. A `ToDictionary()` escape hatch exists for parity tests and diagnostics, marked as allocating. |
| ada-python `replace_url(url, **kwargs)` | `TryReplace(ReadOnlySpan<byte>, in AdaUrlReplacements, Span<byte>, out int)` | `**kwargs` becomes an `in` options struct with per field unset sentinels |
| ada-python `URLSearchParams.items()` | `AdaSearchParams.GetEnumerator()` returning a `ref struct` enumerator | Not exposed as `IEnumerable<T>`, which would box the enumerator and allocate |
| ada-python `idna.encode` and `decode` | `AdaIdna.TryToAscii` and `TryToUnicode`, plus string overloads | Both natives return owned strings, freed inside the wrapper |
| goada `New` and `NewWithBase` | `AdaUrl.TryParse` and `TryParseWithBase` | goada calls the non existent `ada_parse_and_validate`. We use `ada_parse` plus `ada_is_valid`. |
| goada predicates | Same names as C# properties | One to one, suppressed transition leaves |
| goada `runtime.AddCleanup` | `SafeHandle` finalizer | Same guarantee |
| goada `runtime.KeepAlive` | `DangerousAddRef` and `Release`, or `ref struct` frame scoping | The ref struct trades the safety net for zero overhead, deliberately |
| goada `C.GoStringN` copy out | `Encoding.UTF8.GetString`, only in the string tier | Our span tier skips the copy, which is where we beat both reference bindings |

Layout is flat, per ADR-0005:

```
src/Ada.Url/
  Interop/   AdaNative.cs, AdaNativeStructs.cs, Transcode.cs, NativeResolver.cs
  AdaUrl.cs  AdaUrlHandle.cs  AdaSearchParams.cs  AdaIdna.cs  AdaUrlComponents.cs
```

### 3.10 Acceptance criteria

| ID | Criterion |
| --- | --- |
| AC-3.1 | `GC.GetAllocatedBytesForCurrentThread()` delta is exactly 0 over 1,000,000 parse and read all iterations, span in and span out |
| AC-3.2 | Every function in section 3.1 is bound, or its omission is justified in writing |
| AC-3.3 | No caller visible path can skip an owned string free, checked by the leak lane |
| AC-3.4 | `[DisassemblyDiagnoser]` shows a direct native call with no marshalling stub |
| AC-3.5 | `[SuppressGCTransition]` appears only on the listed leaf set, checked in CI |
| AC-3.6 | Public API is captured in `PublicAPI.Shipped.txt`, so every change is a reviewed diff |

---

## 4. Test strategy

### 4.1 Framework and layout

xUnit v3, per ADR-0002.

```
tests/
  Ada.Url.Tests/              unit, conformance, and ABI, separated by trait
  Ada.Url.Tests.Packaging/    Alpine, NativeAOT, and clean install consumption
benchmarks/Ada.Url.Benchmarks/
```

One test project, not six. Traits (`Category=Conformance`, `Abi`, `Stress`) do the separation. A
project per concern is build overhead at this size.

The corpus suite is the bulk of the pyramid, and it is fast and in process. Coverage gate is 80%
line and 70% branch through coverlet. No real PII in test data. URL vectors do
legitimately contain `user:password@` forms, which are corpus literals and must never be replaced
with anything real.

### 4.2 Vectors

| File, from web-platform-tests `url/resources/` | Validates |
| --- | --- |
| `urltestdata.json` | The canonical parse corpus, thousands of input and base pairs with expected components or failure. The most valuable asset in this plan. |
| `setters_tests.json` | Per setter behaviour, which exercises the section 3.2 invalidation rules |
| `IdnaTestV2.json` | UTS-46 and IDNA conformance |
| `toascii.json` | Legacy domain to ASCII cases |
| `percent-encoding.json` | Per component encode sets |
| `urltestdata-javascript-only.json` | Excluded, with a comment saying why. Silent omission is how coverage gaps hide. |

Vendored at a pinned WPT commit under `tests/vectors/`, with the hash in `PROVENANCE.md` and a
weekly CI job that diffs upstream and opens an issue. Never fetched at test time. A JSON to
`[Theory]` adapter turns each vector into a separately named case, so a failure names the input.

**Parity with the reference bindings.** A reflection test asserts the section 3.9 table is
complete, so an upstream addition shows up as a failing test rather than an unnoticed gap. On top
of that, the bindings' own README examples become assertions:

- `'https://example.org/path/../file.txt'` for dot segment removal
- setting `host`, then re-reading `href`
- `URLSearchParams('key1=value1&key2=value2').items()` for order
- `idna.encode('Bücher.example') == b'xn--bcher-kva.example'` and its inverse
- `parse_url('https://user:pass@example.org:80/api?q=1#2')`, including that `:80` is retained,
  because 443 is the https default and 80 is not

These are cheap to write and they catch the class of bug where our wrapper is self consistent but
disagrees with every other Ada binding.

**`System.Uri` is a difference report, never an oracle.** A small curated agreement set is
asserted. Everything else goes into a generated, committed `docs/system-uri-differences.md`. A new
row is a signal to investigate, not automatically a failure.

### 4.3 Vector categories

| # | Category | Concerns |
| --: | --- | --- |
| 1 | Schemes | special and non special, `file`, `ws`, `ftp`, unknown, case normalisation |
| 2 | Default ports | `https://x:443` dropped, `https://x:80` retained |
| 3 | Credentials | `user:pass@`, empty username, `@` in password, `has_*` matrix |
| 4 | IPv4 hosts | dotted decimal, octal, hex, short forms, overflow rejection |
| 5 | IPv6 hosts | brackets, `::`, zone IDs, malformed rejection |
| 6 | Opaque hosts | non special schemes, forbidden host code points |
| 7 | Relative resolution | with base, `//`, `/`, `?`, `#`, empty, scheme relative |
| 8 | Dot segments | `..`, `.`, `%2e`, popping past root |
| 9 | Backslash | `\` becomes `/` for special schemes only |
| 10 | Percent encoding | distinct sets per component, pre encoded input, invalid `%` |
| 11 | Query and search params | serialisation, `sort()` stability, duplicate keys, `+` versus `%20`, `get_all` order |
| 12 | `file:` URLs | Windows drive letters, UNC, empty host, `file:` with base |
| 13 | IDNA | round trip, UTS-46 mapping, bidi, disallowed code points, confusables |
| 14 | `ada_url_omitted` sentinel | every components field checked against `0xFFFFFFFF` on URLs missing each part |
| 15 | Setter invalidation | documented span invalidation after a setter |
| 16 | Whitespace and control | leading and trailing trim, tab, CR, and LF removed from the middle |
| 17 | Max input length | at, below, and above the boundary |
| 18 | UTF-16 parity | every UTF-8 vector re-run through the UTF-16 entry point, identical results |
| 19 | Lone surrogates | unpaired surrogate gives a clean `false`, never an escaping exception |
| 20 | Round trip stability | `parse(href(parse(x))) == parse(x)`, property based |
| 21 | Degenerate input | `""`, `":"`, `"http:"`, `"//"`, `"?"`, `"#"`, very long components |

Categories 14, 15, 18, and 19 exist because of hazards this plan identified. They would not appear
in a straight port of another binding's suite.

### 4.4 Leaks and stress

**Managed allocation assertion, every CI run.**

```csharp
[Fact]
public void ParseAndReadAll_AllocatesZeroBytes()
{
    Warmup(10_000);                                  // JIT plus ArrayPool first touch
    GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
    long before = GC.GetAllocatedBytesForCurrentThread();
    for (int i = 0; i < 1_000_000; i++) { /* span in, span out, parse and read all */ }
    Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
}
```

Warm up before taking the baseline, or the pool's first allocation lands in the measurement.

**Native leaks: Linux ASAN, and that is enough.** Rebuild the native with
`-fsanitize=address,undefined` and run the full conformance suite under it with
`LSAN_OPTIONS=detect_leaks=1`. Any leak or UB report fails the job. Our wrapper code is platform
independent, so a leak found on Linux is a leak everywhere. That is why the first draft's
valgrind, Application Verifier, macOS `leaks`, and RSS slope lanes were cut.

**Handle counters, `ADA_DIAGNOSTICS` builds only.**

```csharp
AdaDiagnostics.LiveUrlHandles / LiveOwnedStrings / LiveSearchParams / LiveIterators
```

Every test class asserts the counters return to their entry values in teardown, which catches a
leaked handle in the fast suite minutes after the commit. The diagnostics build runs in CI
alongside Release.

**Soak.** One million mixed corpus iterations across parse, mutate, read, search params, IDNA, and
copy, asserting no handle imbalance and no exceptions. Nightly, excluded from PR CI by trait so PR
feedback stays fast.

**Property based tests** over transcode and round trip invariants, in the normal suite. This
replaces the fuzzing lane that was cut. Invariants: never crash, never throw an undocumented
exception type, never leak, never return a span outside the native buffer.

**Negative safety.** Double `Dispose` is idempotent. Use after `Dispose` throws
`ObjectDisposedException`. A failed `TryParse` leaks nothing, which is the most commonly missed
leak path, because some C APIs still return a handle on failure.

### 4.5 ABI conformance

The highest risk and lowest visibility failure mode in the design. Runs on every RID.

| Test | Asserts |
| --- | --- |
| `AdaString` layout | `sizeof == 2 * sizeof(nint)`, field offsets 0 and `sizeof(nint)` |
| Blittability | `!RuntimeHelpers.IsReferenceOrContainsReferences<T>()` for every interop struct |
| Struct return by value | `ada_get_href` on a known URL returns a pointer and length that reconstruct the expected bytes exactly. This is the direct test for `sret` versus register pair mishandling. |
| `ada_string_pair` return | Two nested borrowed strings survive `entries_iter_next`, a 32 byte struct return and even likelier to be mishandled |
| `ada_url_components` layout | `sizeof == 32`, every field offset, sentinel round trip |
| `ada_version_components` | Matches `ada_get_version()`, a cheap proof the ABI is aligned at all |
| `size_t` width | A length near `int.MaxValue` does not truncate, memory permitting |
| C `bool` width | `ada_is_valid` returns exactly 0 or 1 in the low byte |

If any of these fails on a platform, that platform does not ship. No workarounds. A silent ABI
mismatch is memory corruption.

### 4.6 Acceptance criteria

| ID | Criterion |
| --- | --- |
| AC-4.1 | All `urltestdata.json` and `setters_tests.json` vectors pass on every RID. No skips without a written justification in `PROVENANCE.md`. |
| AC-4.2 | `IdnaTestV2.json` passes, or each deviation is documented as upstream Ada behaviour with an issue link |
| AC-4.3 | Coverage at or above 80% line and 70% branch |
| AC-4.4 | ASAN and LSAN over the full conformance suite report no leaks and no UB |
| AC-4.5 | One million iteration soak: counters balanced, no exceptions |
| AC-4.6 | ABI suite green on all six RIDs |
| AC-4.7 | `docs/system-uri-differences.md` generated and committed |
| AC-4.8 | Weekly WPT drift job running and filing issues |

---

## 5. Benchmarks

### 5.1 Two questions, and the honesty rule

Is Ada.Url faster than `System.Uri`? That is comparative and needs a fair baseline.

Is it actually zero allocation? That is absolute and needs an assertion, not a measurement.

Every benchmark exists in three tiers, and the zero byte figure is only ever quoted for T1.

| Tier | Shape | Expected `Allocated` |
| --- | --- | --- |
| T1 | UTF-8 span in, span out | 0 B |
| T2 | UTF-8 span in, `string` out | exactly one string |
| T3 | `string` in, `string` out, the fair `System.Uri` comparison | one string plus transcode |

Publishing only T1 while users write T3 code would mislead. Publishing only T3 hides what the
library actually achieves. Both go into `docs/benchmarks/`, side by side.

### 5.2 Harness

```csharp
[MemoryDiagnoser(displayGenColumns: true)]
[DisassemblyDiagnoser(printSource: false, maxDepth: 2)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[CategoriesColumn, MedianColumn]
public class UrlBenchmarks { }
```

Config adds `StatisticColumn.P95`, `MarkdownExporter.GitHub`, `JsonExporter.Full`,
`BaselineRatioColumn`, and `RankColumn`.

Details that make the numbers trustworthy:

- Default out of process toolchain for headline numbers. `InProcessEmitToolchain` shares JIT
  state with the host and biases results.
- Two job variants. Defaults, which is realistic with tiered PGO on, and
  `DOTNET_TieredCompilation=0` with `DOTNET_TieredPGO=0`, which is stable. Publish the realistic
  set, regression track the stable one.
- `OperationsPerInvoke = 1000` on the batch workload, so results read per URL.
- Every benchmark returns a consumed value. A `void` benchmark whose result is unused can be
  eliminated entirely, which produces a spectacular and completely false number.
- `GlobalSetup` pre-transcodes the UTF-8 inputs, so T1 measures parsing and not setup.
- A hardware stamp with every published result. A benchmark number without its hardware is noise.

### 5.3 Workloads

**W1, basic.** `https://example.com/path`. Variants: `CanParse`, parse plus `href`, parse plus all
components through the ten getters, parse plus components through slicing. That last variant is
the input to the section 3.7 decision.

**W2, complex.** Credentials, IDNA, dot segments, and a heavy percent encoded query:

```
https://user:p%40ss@sub.dømain.example.co.uk:8443/a/../b/./c%2Fd/e%20f
  ?q=hello+world&filter[]=1&filter[]=2&token=%E2%9C%93&redirect=https%3A%2F%2Fother.example%2Fx
  #section-2%20anchor
```

Variants: parse plus full read, parse plus search params enumeration, parse plus mutate host plus
re-serialise, and `idna.to_ascii` on the host alone. This is where Ada's SIMD paths and
`System.Uri`'s allocation behaviour diverge most.

**W3, batch of 1,000 mixed payloads.** A seeded fixed array: 300 simple https, 150 with queries,
100 with credentials, 100 IPv4 and IPv6, 100 IDNA, 100 relative with base, 100 `file:` and non
special, 50 pathological. Variants: sequential parse and validate, parse plus extract hostname
which is the real allow list pattern, and `Parallel.For` to show handle per thread scaling.

**W4, transcode isolation.** UTF-16 to UTF-8 across 16, 64, 256, 512, 1024, and 4096 byte buckets.
This quantifies what UTF-16 callers pay and sets the 512 byte `stackalloc` threshold from section
3.4. Its whole purpose is to replace a guessed constant with a measured one.

The native variant comparison, simdutf on versus off versus AVX2, is a one off measurement in P1
that feeds ADR-0003. It is not part of the maintained suite.

### 5.4 Baselines

| Baseline | Compared against | Note |
| --- | --- | --- |
| `new Uri(string)` | T3 parse | `[Benchmark(Baseline = true)]` for T3 |
| `Uri.TryCreate` | T3 `TryParse` | The fair non throwing comparison |
| `Uri` property reads (`Host`, `AbsolutePath`, `Query`) | T2 and T3 component reads | `Uri` caches some and computes others, so read all of them, not one |
| `UriBuilder` round trip | mutate and re-serialise | The only stock analogue of Ada's setters |
| `HttpUtility.ParseQueryString` | `AdaSearchParams` | Query parsing |
| `IdnMapping.GetAscii` | `AdaIdna.TryToAscii` | Footnote the IDNA2003 and 2008 versus UTS-46 difference. It is a spec difference, not a defect. |
| Ada.Url under JIT | Ada.Url under NativeAOT | Confirms the zero allocation path survives AOT |

Published results have to say that `System.Uri` and Ada.Url implement different specifications,
and link to `docs/system-uri-differences.md`. A raw speed comparison between a WHATWG parser and
an RFC 3986 parser is interesting but incomplete on its own.

### 5.5 Columns

`Mean` in ns, `Error`, `StdDev`, `Median`, `P95`, `Ratio`, `RatioSD`, `Gen0`, `Gen1`, `Gen2`,
`Allocated`, `Alloc Ratio`, `Rank`. `Gen0` through `Gen2` and `Allocated` must be zero for T1.
The archived disassembly artifact proves there is no marshalling stub.

### 5.6 Regression detection

BenchmarkDotNet is not a CI gate. Shared runners are too noisy for nanosecond thresholds, and a
flaky perf gate gets disabled within a month, taking real coverage with it.

1. **Allocation is gated in CI as an xUnit test**, using `GC.GetAllocatedBytesForCurrentThread()`.
   Deterministic and fast. This is what makes the zero byte claim enforceable rather than merely
   measured.
2. **Latency is tracked nightly**, archived as JSON and compared to a stored baseline. A
   regression over 15% opens an issue. It never blocks a PR.
3. **Published results** go in `docs/benchmarks/{version}/`, refreshed per release.
4. **Disassembly is archived per release.** A marshalling stub appearing there is a hard
   regression, and it is the machine checkable form of the section 3.4 claim.

### 5.7 Acceptance criteria

| ID | Criterion |
| --- | --- |
| AC-5.1 | W1 through W3 implemented in all three tiers with `System.Uri` baselines |
| AC-5.2 | T1 reports `Allocated = 0 B` and `Gen0 = Gen1 = Gen2 = 0` for W1, W2, and W3 |
| AC-5.3 | `Mean` in ns with `Error`, `StdDev`, `Median`, and `P95` |
| AC-5.4 | The JIT versus NativeAOT job confirms T1 stays at 0 B under AOT |
| AC-5.5 | W4 yields the measured `stackalloc` threshold, and the code constant matches it |
| AC-5.6 | W1's ten getters versus slicing variant decides section 3.7 with data |
| AC-5.7 | Disassembly artifact archived, showing no marshalling stub on T1 |
| AC-5.8 | Nightly perf tracking with a stored baseline and a 15% alert |

---

## 6. CI workflows

Four workflows plus Dependabot. The key design point is that PR CI must not rebuild the native
library. Natives come from a reusable workflow whose output is cached on the Ada tag plus the
build script hash, so the common PR path is a cache hit.

Conventions across all files: least privilege `permissions` per job, `concurrency` groups to
cancel superseded runs, third party actions pinned by full commit SHA, no secrets in logs, and
central package management for restore.

### 6.1 `native.yml`, reusable

```yaml
name: native
on:
  workflow_call:
    inputs:
      ada-tag: { type: string, default: 'v4.0.0' }
  workflow_dispatch:

permissions:
  contents: read

jobs:
  build:
    name: ${{ matrix.rid }}
    runs-on: ${{ matrix.os }}
    strategy:
      fail-fast: false
      matrix:
        include:
          - { rid: win-x64,        os: windows-2022,     script: native/build-windows.ps1 }
          - { rid: linux-x64,      os: ubuntu-24.04,     script: native/build-linux.sh    }
          - { rid: linux-arm64,    os: ubuntu-24.04-arm, script: native/build-linux.sh    }
          - { rid: linux-musl-x64, os: ubuntu-24.04,     script: native/build-musl.sh     }
          - { rid: osx-x64,        os: macos-13,         script: native/build-macos.sh    }
          - { rid: osx-arm64,      os: macos-14,         script: native/build-macos.sh    }
    steps:
      - uses: actions/checkout@v4
      - id: key
        shell: bash
        run: echo "k=native-${{ inputs.ada-tag }}-${{ matrix.rid }}-${{ hashFiles('native/**') }}" >> "$GITHUB_OUTPUT"
      - uses: actions/cache@v4
        id: cache
        with:
          path: artifacts/native/${{ matrix.rid }}
          key: ${{ steps.key.outputs.k }}
      - if: steps.cache.outputs.cache-hit != 'true'
        run: ${{ matrix.script }} --ada-tag ${{ inputs.ada-tag }} --rid ${{ matrix.rid }}
        shell: bash
      # AC-2.2. A library that builds but exports nothing is this section's silent failure.
      - run: native/verify-exports.sh artifacts/native/${{ matrix.rid }}
        shell: bash
      - run: native/verify-hardening.sh artifacts/native/${{ matrix.rid }}   # AC-2.4
        shell: bash
      - run: native/checksum.sh artifacts/native/${{ matrix.rid }} >> artifacts/native/CHECKSUMS.new
        shell: bash
      - uses: actions/upload-artifact@v4
        with: { name: native-${{ matrix.rid }}, path: artifacts/native/${{ matrix.rid }}, retention-days: 7 }
```

### 6.2 `ci.yml`, the PR gate

Jobs: `natives` (calls the reusable workflow), `build-test` (matrix over win-x64, linux-x64,
linux-arm64, osx-arm64, running build, test with coverage, the ABI leg, the `ADA_DIAGNOSTICS`
leg, and the no `#if` grep), `packaging` (pack, Alpine container test, NativeAOT consumption
test), `analyze` (CodeQL), and `deps` (dependency review at `fail-on-severity: high`, per org
standard).

The live file is `.github/workflows/ci.yml`. The `natives` job and the native download are
wired. The ABI leg lands with P2 and the packaging job with P4, both marked with a TODO.

Branch protection on `main`: require `build-test` on all matrix legs,
`packaging`, `analyze`, and `deps`. One approval, two for changes under `src/Ada.Url/Interop/` or
`native/`. No self approval, linear history, no force push.

### 6.3 `nightly.yml`

Three jobs on a 03:00 cron.

`asan` rebuilds the native with `-fsanitize=address,undefined` and runs the conformance suite
under `LSAN_OPTIONS=detect_leaks=1`. This is the primary native leak gate, AC-4.4.

`soak` runs the stress trait with `ADA_DIAGNOSTICS` for AC-4.5.

`bench` runs BenchmarkDotNet with the JSON exporter and compares to the stored baseline at a 15%
tolerance. It opens an issue, it never fails the build. Move it to a pinned self hosted runner for
absolute nanosecond numbers to be worth anything.

A fourth job, `wpt-drift`, diffs the pinned corpus against upstream and opens an issue, AC-4.8.

### 6.4 `release.yml`

Triggered on a `v*` tag. Calls `native.yml`, then a `publish` job on `windows-2022` for code
signing, gated by `environment: production` for the release approval step.

The job verifies checksums (AC-2.6), packs, signs, generates the SBOM, pushes to nuget.org using
trusted publishing with `id-token: write`, and creates the GitHub release with the packages and
SBOM attached.

Release gate: green CI, production environment approval, a CHANGELOG entry, and a rollback plan.
The rollback is to unlist the NuGet version. Packages cannot be deleted, so unlisting is the
rollback and the runbook has to say so. Post-deploy verification is installing the published
package from nuget.org in a clean project on each OS and running the smoke test.

### 6.5 `dependabot.yml`

Weekly updates for the `nuget` and `github-actions` ecosystems. Upstream Ada is not a Dependabot
ecosystem. Its tag moves by hand under the ADR-0003 upgrade policy, with the conformance suite as
the gate.

---

## 7. Repository and roadmap

### 7.1 Layout

```
ada-csharp/
  ADA_WRAPPER_PLAN.md  README.md  CHANGELOG.md  LICENSE  THIRD-PARTY-NOTICES.txt
  Directory.Build.props  Directory.Packages.props  global.json
  .editorconfig  .gitattributes  .gitignore  Ada.Url.slnx
  .claude/skills/writing-style/     house writing rules
  docs/
    adr/                            ADR-0001 to ADR-0005
    benchmarks/{version}/
    runbooks/release.md
    system-uri-differences.md       generated
  native/
    build-windows.ps1  build-linux.sh  build-musl.sh  build-macos.sh
    verify-exports.sh  verify-hardening.sh  checksum.sh  verify-checksums.ps1
    CHECKSUMS.txt
  src/Ada.Url/
    Interop/  AdaNative.cs  AdaNativeStructs.cs  Transcode.cs  NativeResolver.cs
    AdaUrl.cs  AdaUrlHandle.cs  AdaSearchParams.cs  AdaIdna.cs  AdaUrlComponents.cs
    PublicAPI.Shipped.txt  PublicAPI.Unshipped.txt
  tests/Ada.Url.Tests/  tests/Ada.Url.Tests.Packaging/
  benchmarks/Ada.Url.Benchmarks/
  .github/workflows/  .github/dependabot.yml
```

### 7.2 ADRs

| ADR | Decision | Status |
| --- | --- | --- |
| ADR-0001 | Single target `net10.0`, UTF-8 first API, MIT license | accepted |
| ADR-0002 | xUnit v3, pinned WPT corpus, `System.Uri` as a difference report | accepted |
| ADR-0003 | Native build and distribution strategy | proposed, item 2 blocked on the P1 measurement |
| ADR-0004 | Lifetime model, and why use after free is not enforced | accepted |
| ADR-0005 | Flat layout instead of four Clean Architecture projects | accepted |

### 7.3 Risk register

| ID | Risk | Impact | Likelihood | Mitigation |
| --- | --- | :-: | :-: | --- |
| R-1 | Struct return by value mishandled on one platform | Critical, silent corruption | Low | ABI suite per RID. Nothing ships without it green. |
| R-2 | Borrowed pointer used after mutation | Critical, use after free | Medium | Contract, handle free primary API, category 15, ASAN. **Open, not mitigated.** Enforcement was traded for zero overhead, see ADR-0004. |
| R-3 | Handle or owned string leaked | High | Medium | `AdaOwnedString`, the `Try...(Span)` API shape, `ADA_DIAGNOSTICS` counters, ASAN |
| R-4 | Native fails to resolve in a single file bundle, container, or AOT publish | High | Medium | `DllImportResolver`, Alpine and NativeAOT consumption tests |
| R-5 | `-fvisibility=hidden` hides the whole `ada_*` surface | High, ships nothing | Medium | Export gate in CI. Drop the flag rather than patch upstream. |
| R-6 | Upstream Ada v5 breaks the C ABI | Medium | Medium | Pin by tag, ADR-0003 upgrade policy, conformance suite as the gate |
| R-7 | Numbers quoted out of context, such as zero bytes for a string API | Medium, credibility | High | The three tier rule on every published table |
| R-8 | Native binary tampered with in the supply chain | Critical | Low | CI only builds, checksum manifest verified at pack, signing and notarisation, SBOM, SHA pinned actions |

### 7.4 Roadmap

| Phase | Deliverable | Exit |
| --- | --- | --- |
| **P0** done | git init, repo skeleton, build props, editorconfig, `ci.yml`, README, ADR-0001 to ADR-0005 | CI green on the scaffold, lint gate live, ADR-0005 approved |
| **P1** done | `native.yml` producing all six RIDs from v4.0.0, export, hardening, and checksum gates. The simdutf measurement feeding ADR-0003 is still outstanding. | AC-2.1 to AC-2.4, AC-2.6 |
| **P2** done | Full `AdaNative` surface, blittable structs, ABI suite, handle free statics, `AdaUrl` ref struct | AC-1.3, AC-3.1 to AC-3.5, AC-4.6 |
| **P3** next | WPT corpus adapter, categories 1 to 21, `SafeHandle`, `AdaSearchParams`, `AdaIdna`, parity tests, ASAN and soak lanes | AC-1.2, AC-4.1 to AC-4.5, AC-4.7, AC-4.8 |
| **P4** | Benchmarks W1 to W4 in three tiers, packaging, Alpine and NativeAOT tests, signing, SBOM, runbook, `release.yml` | AC-1.1, AC-1.4, AC-1.5, AC-2.5, AC-2.7, AC-5.1 to AC-5.8 |

One target framework is what makes this five phases instead of eight. No fallback lane to build,
no second interop path to keep behaviourally identical, no `#if` to audit.

---

## 8. Open questions

| ID | Question | Recommendation |
| --- | --- | --- |
| O-1 | Ship `win-arm64` in v1? | Defer. One more runner and ABI lane, and Windows on ARM .NET server use is still rare. Easy to add later. |
| O-2 | Which ticket does this hang off? | Work should tie to a ticket, with written testable acceptance criteria before development starts. The AC tables here are written to paste straight in. |
| O-3 | NuGet trusted publishing or a stored API key? | Trusted publishing. `release.yml` already asks for `id-token: write`. It needs a one time policy on nuget.org against this repo, and there is no long lived secret to rotate. |
| O-4 | Is `Ada.Url` free on nuget.org, and is it the right id? | The id has to be free and should not imply an official ada-url project. Consider `AdaUrl.Net` or an owner prefixed id. Worth opening a courtesy issue upstream so the binding can be listed with the Go and Python ones. |

Resolved: single target `net10.0` (ADR-0001), public OSS on nuget.org under MIT, flat layout
approved (ADR-0005), P0 executed.

---

## 9. Notes for the implementer

Two items were checked against `ada_c.h` at v4.0.0. Do not re-litigate them.

**`ada_parse_and_validate` does not exist.** goada references it. The v4.0.0 header does not
declare it. Use `ada_parse` then `ada_is_valid`.

**The absent component sentinel is `#define ada_url_omitted 0xffffffff`,** which is
`uint32_t(-1)`. It is defined at header level, not as a comment on `ada_url_components.port`, and
it applies to every `uint32` field in that struct. See section 3.7 and test category 14.

One documentation trap: `https://raw.githubusercontent.com/ada-url/ada-python/main/README.md`
returns 404. Use `https://github.com/ada-url/ada-python`.

One thing still to verify during implementation: whether upstream annotates its C API for symbol
visibility. If it does not, `-fvisibility=hidden` hides the entire `ada_*` surface and produces a
library that links and exports nothing. AC-2.2 catches it, and the fix is to drop the flag rather
than patch upstream.
