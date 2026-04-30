namespace Payments;

/// <summary>
/// Represents the expiry date printed on a payment card.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CardExpiry"/> struct.
/// </remarks>
/// <param name="month">The expiry month (1–12).</param>
/// <param name="year">The full four-digit expiry year.</param>
public readonly struct CardExpiry(int month, int year)
{
    /// <summary>
    /// Gets a value indicating whether this card has expired as of today's date.
    /// A card is valid through the last day of its expiry month.
    /// </summary>
    public bool IsExpired => this.IsExpiredAsOf(DateOnly.FromDateTime(DateTime.Today));

    /// <summary>
    /// Gets the expiry month (1–12).
    /// </summary>
    public int Month { get; } = month;

    /// <summary>
    /// Gets the full four-digit expiry year.
    /// </summary>
    public int Year { get; } = year;

    /// <summary>
    /// Attempts to parse a card expiry from a string in <c>MMYY</c> or <c>MM/YY</c> format.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the parsed <see cref="CardExpiry"/>.</param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out CardExpiry result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string normalized = value.Trim();

        if (normalized.Length == 5 && (normalized[2] == '/' || normalized[2] == '-'))
        {
            normalized = normalized.Remove(2, 1);
        }

        if (normalized.Length != 4)
        {
            return false;
        }

        if (!int.TryParse(normalized.AsSpan(0, 2), out int month) ||
            !int.TryParse(normalized.AsSpan(2, 2), out int twoDigitYear))
        {
            return false;
        }

        if (month is < 1 or > 12)
        {
            return false;
        }

        result = new CardExpiry(month, 2000 + twoDigitYear);

        return true;
    }

    /// <summary>
    /// Determines whether this card has expired as of the given date.
    /// A card is valid through the last day of its expiry month.
    /// </summary>
    /// <param name="asOf">The date to evaluate expiry against.</param>
    /// <returns><see langword="true"/> if the card has expired; otherwise <see langword="false"/>.</returns>
    public bool IsExpiredAsOf(DateOnly asOf) => asOf >= new DateOnly(this.Year, this.Month, 1).AddMonths(1);

    /// <summary>
    /// Returns the expiry date in <c>MM/YY</c> format.
    /// </summary>
    /// <returns>The expiry date formatted as <c>MM/YY</c>.</returns>
    public override string ToString() => $"{this.Month:D2}/{this.Year % 100:D2}";
}
