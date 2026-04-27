namespace Collections.Specialized;

/// <summary>
/// Defines a lookup that maps digit-only prefixes to values, supporting both read and write operations.
/// </summary>
/// <typeparam name="TKey">The type of the key used for prefix lookup. Must be enumerable of characters.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each prefix.</typeparam>
public interface IPrefixLookup<TKey, TValue> : IReadOnlyPrefixLookup<TKey, TValue>
    where TKey : IEnumerable<char>
{
    /// <summary>
    /// Adds a value to the lookup with the specified digit prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix associated with the value.</param>
    /// <param name="value">The value to be added to the lookup.</param>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="prefix"/> contains non-digit characters, or an element with the same prefix already exists.</exception>
    void Add(TKey prefix, TValue value);

    /// <summary>
    /// Attempts to add a value to the lookup with the specified digit prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix associated with the value.</param>
    /// <param name="value">The value to be added to the lookup.</param>
    /// <returns><see langword="true"/> if the value was added; <see langword="false"/> if the prefix already exists.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="prefix"/> contains non-digit characters.</exception>
    bool TryAdd(TKey prefix, TValue value);
}
