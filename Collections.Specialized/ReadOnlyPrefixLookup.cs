namespace Collections.Specialized;

/// <summary>
/// A read-only decorator that wraps an <see cref="IPrefixLookup{TKey, TValue}"/> and exposes it as an <see cref="IReadOnlyPrefixLookup{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the key used for prefix lookup. Must be enumerable of characters.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each prefix.</typeparam>
public class ReadOnlyPrefixLookup<TKey, TValue> : IReadOnlyPrefixLookup<TKey, TValue>
    where TKey : IEnumerable<char>
{
    private readonly IPrefixLookup<TKey, TValue> inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyPrefixLookup{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="inner">The lookup to wrap.</param>
    public ReadOnlyPrefixLookup(IPrefixLookup<TKey, TValue> inner)
    {
        ArgumentNullException.ThrowIfNull(inner);
        this.inner = inner;
    }

    /// <inheritdoc/>
    public int Count => this.inner.Count;

    /// <inheritdoc/>
    public TValue this[TKey prefix]
    {
        get
        {
            if (!this.inner.TryGetValue(prefix, out var value))
            {
                throw new PrefixNotFoundException("The specified prefix was not found.");
            }

            return value;
        }
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey prefix, out TValue value) => this.inner.TryGetValue(prefix, out value);
}
