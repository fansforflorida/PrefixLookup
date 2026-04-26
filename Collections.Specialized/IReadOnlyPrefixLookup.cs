namespace Collections.Specialized;

/// <summary>
/// A read-only view of a lookup that maps digit-only prefixes to values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TKey">The type of the key used for prefix lookup. Must be enumerable of characters.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each prefix.</typeparam>
public interface IReadOnlyPrefixLookup<in TKey, TValue>
    where TKey : IEnumerable<char>
{
    /// <summary>Gets the number of prefix/value pairs in the lookup.</summary>
    int Count { get; }

    /// <summary>
    /// Gets the value associated with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to look up.</param>
    /// <returns>The value associated with the specified prefix.</returns>
    /// <exception cref="PrefixNotFoundException">The specified prefix was not found.</exception>
    TValue this[TKey prefix] { get; }

    /// <summary>
    /// Attempts to get the value associated with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to look up.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified prefix, if the prefix is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the prefix was found; otherwise, <see langword="false"/>.</returns>
    bool TryGetValue(TKey prefix, out TValue value);
}
