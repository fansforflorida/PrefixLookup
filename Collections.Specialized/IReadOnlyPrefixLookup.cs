namespace Collections.Specialized;

/// <summary>
/// A read-only view of a lookup that maps digit-only prefixes to values of type <typeparamref name="TValue"/>.
/// </summary>
/// <remarks>
/// This interface represents a read-only <em>view through this reference</em>. It does not guarantee
/// that the underlying collection is immutable. A caller holding an
/// <see cref="IPrefixLookup{TKey, TValue}"/> reference to the same instance may still modify it.
/// </remarks>
/// <typeparam name="TKey">The type of the key used for prefix lookup. Must be enumerable of characters.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each prefix.</typeparam>
public interface IReadOnlyPrefixLookup<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    where TKey : IEnumerable<char>
{
    /// <summary>Gets an enumerable sequence of all keys in the lookup.</summary>
    IEnumerable<TKey> Keys { get; }

    /// <summary>Gets an enumerable sequence of all values in the lookup.</summary>
    IEnumerable<TValue> Values { get; }

    /// <summary>
    /// Gets the value associated with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to look up.</param>
    /// <returns>The value associated with the specified prefix.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="PrefixNotFoundException">The specified prefix was not found.</exception>
    TValue this[TKey prefix] { get; }

    /// <summary>
    /// Attempts to get the value associated with the longest registered prefix that matches the start of <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The key to look up. The method scans this value from left to right and returns the value for the longest registered prefix found.</param>
    /// <param name="value">When this method returns, contains the value associated with the longest matching prefix, if one is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if a matching prefix was found; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    bool TryGetValue(TKey prefix, out TValue value);
}
