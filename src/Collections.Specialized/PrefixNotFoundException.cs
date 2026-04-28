namespace Collections.Specialized;

/// <summary>
/// Exception thrown when a prefix is not found in a prefix lookup.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PrefixNotFoundException"/> class with a specified error message.
/// </remarks>
/// <param name="message">The message that describes the error.</param>
public class PrefixNotFoundException(string? message) : KeyNotFoundException(message)
{
}
