namespace Collections.Specialized;

using System.Collections;
using System.Text;

/// <summary>
/// A trie-based lookup that maps digit-only prefixes to values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the values stored in the lookup.</typeparam>
public class DigitPrefixLookup<TValue> : IPrefixLookup<string, TValue>
{
    private readonly Node root = new();

    /// <inheritdoc/>
    public int Count { get; internal set; }

    /// <inheritdoc/>
    public IEnumerable<string> Keys => this.Select(kvp => kvp.Key);

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => this.Select(kvp => kvp.Value);

    /// <summary>
    /// Gets or sets a value by its digit prefix.
    /// </summary>
    /// <param name="prefix">The digit prefix associated with the value.</param>
    /// <returns>The value associated with the specified prefix.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="prefix"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The setter: <paramref name="prefix"/> contains non-digit characters, or an element with the same prefix already exists.</exception>
    /// <exception cref="PrefixNotFoundException">The getter: the specified prefix was not found.</exception>
    public TValue this[string prefix]
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

    /// <inheritdoc/>
    public void Add(string prefix, TValue value)
    {
        if (!this.TryAdd(prefix, value))
        {
            throw new ArgumentException("An element with the same prefix already exists.", nameof(prefix));
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentException"><paramref name="prefix"/> contains non-digit characters.</exception>
    public bool TryAdd(string prefix, TValue value)
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
    public bool TryGetValue(string prefix, out TValue value)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        Node node = this.root;
        TValue lastMatch = default!;
        bool found = false;

        foreach (char c in prefix)
        {
            int i = c - '0';

            if (i < 0 || i > 9 || node.Children[i] == null)
            {
                break;
            }

            node = node.Children[i]!;

            if (node.HasValue)
            {
                lastMatch = node.Value;
                found = true;
            }
        }

        value = lastMatch;
        return found;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
    {
        StringBuilder sb = new();

        // Stack frame: (node, nextChildIndex, prefixLength)
        var stack = new Stack<(Node node, int nextChild, int prefixLength)>();

        stack.Push((this.root, 0, 0));

        while (stack.Count > 0)
        {
            var (node, nextChild, prefixLength) = stack.Pop();

            // Restore the prefix to what it was when we entered this node
            sb.Length = prefixLength;

            if (node.HasValue)
            {
                yield return new KeyValuePair<string, TValue>(sb.ToString(), node.Value);
            }

            for (int digit = nextChild; digit < 10; digit++)
            {
                Node? child = node.Children[digit];

                if (child != null)
                {
                    // Append this digit to the prefix
                    sb.Append((char)('0' + digit));

                    // Push a continuation frame for the current node
                    // (resume scanning children after this one)
                    stack.Push((node, digit + 1, prefixLength));

                    // Push a frame for the child node
                    stack.Push((child, 0, sb.Length));

                    // Depth‑first: go down immediately
                    break;
                }
            }
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    private sealed class Node
    {
#pragma warning disable SA1401 // Fields should be private
        public readonly Node?[] Children = new Node?[10];
        public bool HasValue;
        public TValue Value = default!;
#pragma warning restore SA1401 // Fields should be private
    }
}
