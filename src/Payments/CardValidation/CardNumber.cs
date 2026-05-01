namespace Payments.CardValidation;

using Payments.CardNetworks;

/// <summary>
/// Represents a payment card number (Primary Account Number / PAN).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CardNumber"/> struct.
/// </remarks>
/// <param name="value">The raw card number digit string.</param>
public readonly struct CardNumber(string value)
{
    private readonly string? value = value;

    /// <summary>
    /// Gets the raw card number value.
    /// </summary>
    public string Value => this.value ?? string.Empty;

    /// <summary>
    /// Gets the card network identified from the card number's BIN prefix.
    /// </summary>
    public IssuingNetwork Network =>
        BinNetworkLookup.Instance.TryGetNetwork(this.Value, out var network)
            ? network
            : IssuingNetwork.Unknown;

    /// <summary>
    /// Gets a value indicating whether the card number passes the Luhn check.
    /// </summary>
    public bool IsLuhnValid => LuhnValidator.IsValid(this.Value);

    /// <summary>
    /// Gets a value indicating whether the card number length is valid for its identified network.
    /// </summary>
    public bool IsValidLength
    {
        get
        {
            int len = this.Value.Length;

            return this.Network switch
            {
                IssuingNetwork.Visa => len is 13 or 16,
                IssuingNetwork.MasterCard => len is 16,
                IssuingNetwork.AmericanExpress => len is 15,
                IssuingNetwork.Discover => len is 16,
                IssuingNetwork.DinersClub => len is 14,
                IssuingNetwork.Jcb => len is 16,
                IssuingNetwork.UnionPay => len is >= 16 and <= 19,
                _ => len is >= 12 and <= 19,
            };
        }
    }

    /// <summary>
    /// Gets the card number with all but the last four digits replaced with asterisks.
    /// </summary>
    public string Masked
    {
        get
        {
            string v = this.Value;

            if (v.Length <= 4)
            {
                return new string('*', v.Length);
            }

            return string.Concat(new string('*', v.Length - 4), v.AsSpan(v.Length - 4));
        }
    }

    /// <summary>
    /// Returns the raw card number value.
    /// </summary>
    /// <returns>The raw card number digit string.</returns>
    public override string ToString() => this.Value;
}
