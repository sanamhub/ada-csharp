using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ada.Url.Interop;

/// <summary>
/// The only point of contact with the native Ada library.
/// </summary>
/// <remarks>
/// <para>
/// Rules for every member here. Parameters and returns are blittable only: <c>byte*</c>,
/// <see cref="nint"/>, <see cref="nuint"/>, <see cref="uint"/>, <see cref="byte"/>, and explicit
/// layout blittable structs. C <c>size_t</c> maps to <see cref="nuint"/> and never to
/// <see cref="int"/>, because a wrong width corrupts the stack on one platform and passes tests
/// on another. C <c>bool</c> maps to <see cref="byte"/>. Always set <c>ExactSpelling</c> and an
/// explicit <c>CallConvCdecl</c>.
/// </para>
/// <para>
/// <c>[SuppressGCTransition]</c> is allowed only on calls that cannot block, allocate, take a
/// lock, or call back into managed code. The permitted set is the nine predicates,
/// <c>ada_is_valid</c>, the two type getters, <c>ada_get_max_input_length</c>, and the ten
/// borrowed string getters. It must never appear on the parse functions, any setter, or either
/// IDNA function. A suppressed transition delays GC for the length of the call, and putting it on
/// a long call causes pauses that are very hard to trace back.
/// </para>
/// </remarks>
internal static unsafe partial class AdaNative
{
    /// <summary>
    /// Base name of the native library. The platform loader adds the prefix and suffix, giving
    /// ada.dll, libada.so, or libada.dylib. This is why the Linux artifact has to be named
    /// exactly libada.so.
    /// </summary>
    internal const string LibraryName = "ada";

    /// <summary>
    /// The <c>ada_url_omitted</c> sentinel, checked against <c>ada_c.h</c> at upstream tag
    /// v4.0.0. Every <see cref="uint"/> field of the components struct can carry it, not just
    /// port. Casting it to <see cref="int"/> without checking gives -1 and an out of range slice.
    /// </summary>
    internal const uint Omitted = 0xFFFFFFFFu;

    /// <summary>Converts a C <c>bool</c> return to a managed one.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ToBool(byte value) => value != 0;

    // ---------------------------------------------------------------------------------------
    // Lifetime
    // ---------------------------------------------------------------------------------------

    [LibraryImport(LibraryName, EntryPoint = "ada_parse")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint Parse(byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_parse_with_base")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint ParseWithBase(byte* input, nuint inputLength, byte* baseInput, nuint baseLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_can_parse")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte CanParse(byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_can_parse_with_base")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte CanParseWithBase(byte* input, nuint inputLength, byte* baseInput, nuint baseLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_free")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Free(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_free_owned_string")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void FreeOwnedString(AdaOwnedString owned);

    [LibraryImport(LibraryName, EntryPoint = "ada_copy")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint Copy(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_is_valid")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte IsValid(nint url);

    // ---------------------------------------------------------------------------------------
    // Borrowed getters. The returned pointer dangles after any setter or free on the same URL.
    // ---------------------------------------------------------------------------------------

    [LibraryImport(LibraryName, EntryPoint = "ada_get_href")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetHref(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_username")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetUsername(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_password")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetPassword(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_port")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetPort(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_hash")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetHash(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_host")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetHost(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_hostname")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetHostname(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_pathname")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetPathname(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_search")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetSearch(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_protocol")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString GetProtocol(nint url);

    /// <summary>Returns owned memory. The caller must release it with <see cref="FreeOwnedString"/>.</summary>
    [LibraryImport(LibraryName, EntryPoint = "ada_get_origin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaOwnedString GetOrigin(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_host_type")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte GetHostType(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_scheme_type")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte GetSchemeType(nint url);

    /// <summary>
    /// Returns a pointer to internal state. It is invalidated by any setter or free on the same
    /// URL, so read it and slice immediately.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "ada_get_components")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaUrlComponentsNative* GetComponents(nint url);

    // ---------------------------------------------------------------------------------------
    // Setters. Each one invalidates every span previously returned from the same URL.
    // No SuppressGCTransition: these reparse and can allocate.
    // ---------------------------------------------------------------------------------------

    [LibraryImport(LibraryName, EntryPoint = "ada_set_href")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetHref(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_host")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetHost(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_hostname")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetHostname(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_protocol")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetProtocol(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_username")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetUsername(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_password")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetPassword(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_port")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetPort(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_pathname")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SetPathname(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_search")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetSearch(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_set_hash")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetHash(nint url, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_clear_port")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void ClearPort(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_clear_hash")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void ClearHash(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_clear_search")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void ClearSearch(nint url);

    // ---------------------------------------------------------------------------------------
    // Predicates. All leaves.
    // ---------------------------------------------------------------------------------------

    [LibraryImport(LibraryName, EntryPoint = "ada_has_credentials")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasCredentials(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_empty_hostname")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasEmptyHostname(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_hostname")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasHostname(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_non_empty_username")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasNonEmptyUsername(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_non_empty_password")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasNonEmptyPassword(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_port")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasPort(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_password")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasPassword(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_hash")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasHash(nint url);

    [LibraryImport(LibraryName, EntryPoint = "ada_has_search")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte HasSearch(nint url);

    // ---------------------------------------------------------------------------------------
    // IDNA. Both return owned memory.
    // ---------------------------------------------------------------------------------------

    [LibraryImport(LibraryName, EntryPoint = "ada_idna_to_unicode")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaOwnedString IdnaToUnicode(byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_idna_to_ascii")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaOwnedString IdnaToAscii(byte* input, nuint length);

    // ---------------------------------------------------------------------------------------
    // Search params. Bound now, wrapped in P3.
    // ---------------------------------------------------------------------------------------

    [LibraryImport(LibraryName, EntryPoint = "ada_parse_search_params")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint ParseSearchParams(byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_free_search_params")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void FreeSearchParams(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_size")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial nuint SearchParamsSize(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_sort")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SearchParamsSort(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_to_string")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaOwnedString SearchParamsToString(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_append")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SearchParamsAppend(nint parameters, byte* key, nuint keyLength, byte* value, nuint valueLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_set")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SearchParamsSet(nint parameters, byte* key, nuint keyLength, byte* value, nuint valueLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_remove")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SearchParamsRemove(nint parameters, byte* key, nuint keyLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_remove_value")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SearchParamsRemoveValue(nint parameters, byte* key, nuint keyLength, byte* value, nuint valueLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_has")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SearchParamsHas(nint parameters, byte* key, nuint keyLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_has_value")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte SearchParamsHasValue(nint parameters, byte* key, nuint keyLength, byte* value, nuint valueLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_get")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaString SearchParamsGet(nint parameters, byte* key, nuint keyLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_get_all")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint SearchParamsGetAll(nint parameters, byte* key, nuint keyLength);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_reset")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SearchParamsReset(nint parameters, byte* input, nuint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_get_keys")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint SearchParamsGetKeys(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_get_values")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint SearchParamsGetValues(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_get_entries")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial nint SearchParamsGetEntries(nint parameters);

    [LibraryImport(LibraryName, EntryPoint = "ada_free_strings")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void FreeStrings(nint strings);

    [LibraryImport(LibraryName, EntryPoint = "ada_strings_size")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial nuint StringsSize(nint strings);

    [LibraryImport(LibraryName, EntryPoint = "ada_strings_get")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial AdaString StringsGet(nint strings, nuint index);

    [LibraryImport(LibraryName, EntryPoint = "ada_free_search_params_keys_iter")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void FreeKeysIter(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_keys_iter_next")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaString KeysIterNext(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_keys_iter_has_next")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte KeysIterHasNext(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_free_search_params_values_iter")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void FreeValuesIter(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_values_iter_next")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaString ValuesIterNext(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_values_iter_has_next")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte ValuesIterHasNext(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_free_search_params_entries_iter")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void FreeEntriesIter(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_entries_iter_next")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaStringPair EntriesIterNext(nint iterator);

    [LibraryImport(LibraryName, EntryPoint = "ada_search_params_entries_iter_has_next")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial byte EntriesIterHasNext(nint iterator);

    // ---------------------------------------------------------------------------------------
    // Global configuration and version
    // ---------------------------------------------------------------------------------------

    /// <summary>
    /// Process wide. It affects every consumer in the process, including other libraries, so set
    /// it once at startup and never per call.
    /// </summary>
    [LibraryImport(LibraryName, EntryPoint = "ada_set_max_input_length")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetMaxInputLength(uint length);

    [LibraryImport(LibraryName, EntryPoint = "ada_get_max_input_length")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    [SuppressGCTransition]
    internal static partial uint GetMaxInputLength();

    /// <summary>Returns a pointer to a static, null terminated string owned by the library.</summary>
    [LibraryImport(LibraryName, EntryPoint = "ada_get_version")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial byte* GetVersion();

    [LibraryImport(LibraryName, EntryPoint = "ada_get_version_components")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial AdaVersionComponents GetVersionComponents();
}
