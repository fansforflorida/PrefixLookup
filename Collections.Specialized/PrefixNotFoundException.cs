namespace Collections.Specialized;

/// <summary>
/// Exception thrown when a prefix is not found in a prefix lookup.
/// </summary>
public class PrefixNotFoundException : KeyNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrefixNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PrefixNotFoundException(string? message)
        : base(message)
    {
    }
}
