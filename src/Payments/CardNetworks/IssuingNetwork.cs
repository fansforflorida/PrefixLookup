namespace Payments.CardNetworks;

/// <summary>
/// Identifies the card network that issued a payment card.
/// </summary>
public enum IssuingNetwork
{
    /// <summary>The network could not be determined from the BIN.</summary>
    Unknown = 0,

    /// <summary>Visa.</summary>
    Visa,

    /// <summary>Mastercard.</summary>
    MasterCard,

    /// <summary>American Express.</summary>
    AmericanExpress,

    /// <summary>Discover.</summary>
    Discover,

    /// <summary>Diners Club.</summary>
    DinersClub,

    /// <summary>JCB.</summary>
    Jcb,

    /// <summary>UnionPay.</summary>
    UnionPay,
}
