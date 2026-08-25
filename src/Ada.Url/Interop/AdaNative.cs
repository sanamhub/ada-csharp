namespace Ada.Url.Interop;

/// <summary>
/// The only point of contact with the native Ada library.
/// </summary>
/// <remarks>
/// <para>
/// Rules for every member here, from ADA_WRAPPER_PLAN.md section 3.3. Parameters and returns
/// are blittable only: <c>byte*</c>, <see cref="nint"/>, <see cref="nuint"/>,
/// <see cref="uint"/>, <see cref="byte"/>, and explicit layout blittable structs. C
/// <c>size_t</c> maps to <see cref="nuint"/> and never to <see cref="int"/>, because a wrong
/// width corrupts the stack on one platform and passes tests on another. C <c>bool</c> maps to
/// <see cref="byte"/>. Always set <c>ExactSpelling</c> and an explicit <c>CallConvCdecl</c>.
/// </para>
/// <para>
/// <c>[SuppressGCTransition]</c> is allowed only on calls that cannot block, allocate, take a
/// lock, or call back into managed code. The permitted set is the nine <c>ada_has_*</c>
/// predicates, <c>ada_is_valid</c>, <c>ada_get_host_type</c>, <c>ada_get_scheme_type</c>,
/// <c>ada_get_max_input_length</c>, and the ten borrowed string getters. It must never appear
/// on <c>ada_parse</c>, <c>ada_parse_with_base</c>, any <c>ada_set_*</c>, or either IDNA
/// function. A suppressed transition delays GC for the length of the call, and putting it on a
/// long call causes pauses that are very hard to trace back.
/// </para>
/// </remarks>
internal static partial class AdaNative
{
    /// <summary>
    /// Base name of the native library. The platform loader adds the prefix and suffix, giving
    /// ada.dll, libada.so, or libada.dylib. This is why the Linux artifact has to be named
    /// exactly libada.so.
    /// </summary>
    internal const string LibraryName = "ada";

    /// <summary>
    /// The <c>ada_url_omitted</c> sentinel, checked against <c>ada_c.h</c> at upstream tag
    /// v4.0.0. Every <see cref="uint"/> field of <c>ada_url_components</c> can carry it, not
    /// just <c>port</c>. Casting it to <see cref="int"/> without checking gives -1 and an out
    /// of range slice.
    /// </summary>
    internal const uint Omitted = 0xFFFFFFFFu;

    // P/Invoke declarations land in P2. ADA_WRAPPER_PLAN.md section 3.1 has the full ABI.
}
