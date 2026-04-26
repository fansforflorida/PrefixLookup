namespace Collections.Specialized;

/// <summary>
/// A trie-based lookup that maps digit-only prefixes to values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the values stored in the lookup.</typeparam>
public class DigitPrefixLookup<TValue> : IPrefixLookup<IEnumerable<char>, TValue>
{
    private readonly Node root = new();

    /// <inheritdoc/>
    public int Count { get; internal set; }

    /// <summary>
    /// Gets or sets a value by its digit prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix associated with the value.</param>
    /// <returns>The value associated with the specified prefix.</returns>
    /// <exception cref="PrefixNotFoundException">The getter: the specified prefix was not found.</exception>
    /// <exception cref="ArgumentException">The setter: an element with the same prefix already exists.</exception>
    public TValue this[IEnumerable<char> prefix]
    {
        get
        {
            if (!this.TryGetValue(prefix, out var value))
            {
                throw new PrefixNotFoundException("The specified prefix was not found.");
            }

            return value;
        }

        set
        {
            this.Add(prefix, value);
        }
    }

    /// <summary>
    /// Returns a read-only decorator wrapping this lookup.
    /// </summary>
    /// <returns>An <see cref="IReadOnlyPrefixLookup{TKey, TValue}"/> view of this lookup.</returns>
    public IReadOnlyPrefixLookup<IEnumerable<char>, TValue> AsReadOnly() =>
        new ReadOnlyPrefixLookup<IEnumerable<char>, TValue>(this);

    /// <inheritdoc/>
    public void Add(IEnumerable<char> prefix, TValue value)
    {
        if (!this.TryAdd(prefix, value))
        {
            throw new ArgumentException("An element with the same prefix already exists.", nameof(prefix));
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="prefix"/> contains non-digit characters.</exception>
    public bool TryAdd(IEnumerable<char> prefix, TValue value)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (!prefix.All(c => c >= '0' && c <= '9'))
        {
            throw new ArgumentException("Contains non-digit characters", nameof(prefix));
        }

        Node node = this.root;

        foreach (char c in prefix)
        {
            int i = c - '0';
            node.Children[i] ??= new Node();
            node = node.Children[i]!;
        }

        if (node.HasValue)
        {
            return false;
        }

        node.HasValue = true;
        node.Value = value;
        this.Count++;
        return true;
    }

    /// <inheritdoc/>
    public bool TryGetValue(IEnumerable<char> prefix, out TValue value)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        Node node = this.root;

        foreach (char c in prefix)
        {
            int i = c - '0';

            if (i < 0 || i > 9 || node.Children[i] == null)
            {
                value = default!;
                return false;
            }

            node = node.Children[i]!;

            if (node.HasValue)
            {
                value = node.Value;
                return true;
            }
        }

        value = default!;
        return false;
    }

    private sealed class Node
    {
#pragma warning disable SA1401 // Fields should be private
        public readonly Node?[] Children = new Node?[10];
        public bool HasValue;
        public TValue Value = default!;
#pragma warning restore SA1401 // Fields should be private
    }
}
