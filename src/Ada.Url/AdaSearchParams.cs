using System.Text;
using Ada.Url.Interop;

namespace Ada.Url;

/// <summary>
/// A parsed query string, matching the WHATWG URLSearchParams interface.
/// </summary>
/// <remarks>
/// <para>
/// Owns native memory, so it must be disposed. Being a ref struct keeps it on the stack, which
/// means the compiler will not let it outlive the enclosing block.
/// </para>
/// <code>
/// using var parameters = AdaSearchParams.Parse("key1=value1&amp;key2=value2"u8);
/// foreach (var entry in parameters)
/// {
///     // entry.Key and entry.Value are borrowed spans, valid until the next mutation.
/// }
/// </code>
/// <para>
/// Enumeration goes through a struct enumerator rather than IEnumerable. Exposing the interface
/// would box the enumerator on every foreach, which is exactly the allocation this library exists
/// to avoid.
/// </para>
/// <para>
/// Every span returned here points at native memory and is invalidated by any append, set,
/// remove, sort, or reset on the same instance.
/// </para>
/// </remarks>
public ref struct AdaSearchParams : IDisposable
{
    private nint _handle;

    private AdaSearchParams(nint handle) => _handle = handle;

    /// <summary>Parses a query string, with or without a leading question mark.</summary>
    /// <param name="utf8Query">The query, as UTF-8 bytes.</param>
    /// <returns>The parsed parameters.</returns>
    public static unsafe AdaSearchParams Parse(ReadOnlySpan<byte> utf8Query)
    {
        fixed (byte* p = utf8Query)
        {
            return new AdaSearchParams(AdaNative.ParseSearchParams(p, (nuint)utf8Query.Length));
        }
    }

    /// <summary>How many key and value pairs are present, counting duplicate keys separately.</summary>
    public readonly int Count => (int)AdaNative.SearchParamsSize(_handle);

    /// <summary>Adds a pair, keeping any existing pair with the same key.</summary>
    /// <param name="utf8Key">The key, as UTF-8 bytes.</param>
    /// <param name="utf8Value">The value, as UTF-8 bytes.</param>
    public readonly unsafe void Append(ReadOnlySpan<byte> utf8Key, ReadOnlySpan<byte> utf8Value)
    {
        fixed (byte* k = utf8Key)
        fixed (byte* v = utf8Value)
        {
            AdaNative.SearchParamsAppend(_handle, k, (nuint)utf8Key.Length, v, (nuint)utf8Value.Length);
        }
    }

    /// <summary>Sets a key to a single value, removing any other pairs with that key.</summary>
    /// <param name="utf8Key">The key, as UTF-8 bytes.</param>
    /// <param name="utf8Value">The value, as UTF-8 bytes.</param>
    public readonly unsafe void Set(ReadOnlySpan<byte> utf8Key, ReadOnlySpan<byte> utf8Value)
    {
        fixed (byte* k = utf8Key)
        fixed (byte* v = utf8Value)
        {
            AdaNative.SearchParamsSet(_handle, k, (nuint)utf8Key.Length, v, (nuint)utf8Value.Length);
        }
    }

    /// <summary>Removes every pair with the given key.</summary>
    /// <param name="utf8Key">The key, as UTF-8 bytes.</param>
    public readonly unsafe void Remove(ReadOnlySpan<byte> utf8Key)
    {
        fixed (byte* k = utf8Key)
        {
            AdaNative.SearchParamsRemove(_handle, k, (nuint)utf8Key.Length);
        }
    }

    /// <summary>Reports whether any pair has the given key.</summary>
    /// <param name="utf8Key">The key, as UTF-8 bytes.</param>
    /// <returns>True when the key is present.</returns>
    public readonly unsafe bool Has(ReadOnlySpan<byte> utf8Key)
    {
        fixed (byte* k = utf8Key)
        {
            return AdaNative.ToBool(AdaNative.SearchParamsHas(_handle, k, (nuint)utf8Key.Length));
        }
    }

    /// <summary>Gets the first value for a key.</summary>
    /// <param name="utf8Key">The key, as UTF-8 bytes.</param>
    /// <returns>The value as a borrowed span, empty when the key is absent.</returns>
    public readonly unsafe ReadOnlySpan<byte> Get(ReadOnlySpan<byte> utf8Key)
    {
        fixed (byte* k = utf8Key)
        {
            return AdaNative.SearchParamsGet(_handle, k, (nuint)utf8Key.Length).AsSpan();
        }
    }

    /// <summary>Sorts the pairs by key, keeping the relative order of equal keys.</summary>
    public readonly void Sort() => AdaNative.SearchParamsSort(_handle);

    /// <summary>Serialises back to a query string, writing into a caller supplied buffer.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="written">Bytes written when this returns true.</param>
    /// <returns>False when the destination is too small.</returns>
    public readonly bool TryToString(Span<byte> destination, out int written)
    {
        AdaOwnedString owned = AdaNative.SearchParamsToString(_handle);
        try
        {
            ReadOnlySpan<byte> span = owned.AsSpan();
            if (span.Length > destination.Length)
            {
                written = 0;
                return false;
            }

            span.CopyTo(destination);
            written = span.Length;
            return true;
        }
        finally
        {
            AdaNative.FreeOwnedString(owned);
        }
    }

    /// <summary>Serialises back to a query string.</summary>
    /// <returns>The query string.</returns>
    /// <remarks>Allocates. Use <see cref="TryToString"/> on a hot path.</remarks>
    public override readonly string ToString()
    {
        AdaOwnedString owned = AdaNative.SearchParamsToString(_handle);
        try
        {
            return Encoding.UTF8.GetString(owned.AsSpan());
        }
        finally
        {
            AdaNative.FreeOwnedString(owned);
        }
    }

    /// <summary>Enumerates the key and value pairs in order.</summary>
    /// <returns>A struct enumerator that allocates nothing.</returns>
    public readonly Enumerator GetEnumerator() => new(AdaNative.SearchParamsGetEntries(_handle));

    /// <summary>Releases the native parameters. Safe to call more than once.</summary>
    public void Dispose()
    {
        nint handle = _handle;
        _handle = nint.Zero;
        if (handle != nint.Zero)
        {
            AdaNative.FreeSearchParams(handle);
        }
    }

    /// <summary>One key and value pair, as borrowed spans.</summary>
    public readonly ref struct Entry
    {
        internal Entry(AdaStringPair pair)
        {
            Key = pair.Key.AsSpan();
            Value = pair.Value.AsSpan();
        }

        /// <summary>The key.</summary>
        public ReadOnlySpan<byte> Key { get; }

        /// <summary>The value.</summary>
        public ReadOnlySpan<byte> Value { get; }
    }

    /// <summary>
    /// Walks the pairs without allocating.
    /// </summary>
    /// <remarks>
    /// The native iterator is a handle that has to be freed, which is why this is disposable and
    /// why foreach over an <see cref="AdaSearchParams"/> must not be abandoned partway by
    /// anything that skips the enumerator's disposal.
    /// </remarks>
    public ref struct Enumerator : IDisposable
    {
        private nint _iterator;
        private AdaStringPair _current;

        internal Enumerator(nint iterator)
        {
            _iterator = iterator;
            _current = default;
        }

        /// <summary>The pair at the current position.</summary>
        public readonly Entry Current => new(_current);

        /// <summary>Advances to the next pair.</summary>
        /// <returns>False when there are no more pairs.</returns>
        public bool MoveNext()
        {
            if (_iterator == nint.Zero || !AdaNative.ToBool(AdaNative.EntriesIterHasNext(_iterator)))
            {
                return false;
            }

            _current = AdaNative.EntriesIterNext(_iterator);
            return true;
        }

        /// <summary>Releases the native iterator.</summary>
        public void Dispose()
        {
            nint iterator = _iterator;
            _iterator = nint.Zero;
            if (iterator != nint.Zero)
            {
                AdaNative.FreeEntriesIter(iterator);
            }
        }
    }
}
