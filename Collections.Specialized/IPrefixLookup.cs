namespace Collections.Specialized;

/// <summary>
/// Defines a lookup that maps digit-only prefixes to values, supporting both read and write operations.
/// </summary>
/// <typeparam name="TKey">The type of the key used for prefix lookup. Must be enumerable of characters.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each prefix.</typeparam>
public interface IPrefixLookup<in TKey, TValue>
    where TKey : IEnumerable<char>
{
    /// <summary>Gets the number of prefix/value pairs in the lookup.</summary>
    int Count { get; }

    /// <summary>
    /// Adds a value to the lookup with the specified digit prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix associated with the value.</param>
    /// <param name="value">The value to be added to the lookup.</param>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An element with the same prefix already exists.</exception>
    void Add(TKey prefix, TValue value);

    /// <summary>
    /// Attempts to add a value to the lookup with the specified digit prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix associated with the value.</param>
    /// <param name="value">The value to be added to the lookup.</param>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <returns><see langword="true"/> if the value was added; <see langword="false"/> if the prefix already exists.</returns>
    bool TryAdd(TKey prefix, TValue value);

    /// <summary>
    /// Attempts to get the value associated with the specified prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix to look up.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified prefix, if the prefix is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <returns><see langword="true"/> if the prefix was found; otherwise, <see langword="false"/>.</returns>
    bool TryGetValue(TKey prefix, out TValue value);
}
