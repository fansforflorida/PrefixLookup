namespace Payments.CardNetworks;

using Collections.Specialized;

/// <summary>
/// Identifies the card network for a given Bank Identification Number (BIN) prefix.
/// </summary>
public sealed class BinNetworkLookup
{
    private readonly IPrefixLookup<string, IssuingNetwork> lookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinNetworkLookup"/> class.
    /// </summary>
    public BinNetworkLookup()
    {
        this.lookup = new DigitPrefixLookup<IssuingNetwork>();

        this.LoadVisa();
        this.LoadMasterCard();
        this.LoadAmericanExpress();
        this.LoadDiscover();
        this.LoadDinersClub();
        this.LoadJcb();
        this.LoadUnionPay();
    }

    /// <summary>
    /// Attempts to identify the card network for the given BIN.
    /// </summary>
    /// <param name="bin">The Bank Identification Number (or any leading digits of a card number) to look up.</param>
    /// <param name="network">When this method returns <see langword="true"/>, contains the matched <see cref="IssuingNetwork"/>; otherwise <see cref="IssuingNetwork.Unknown"/>.</param>
    /// <returns><see langword="true"/> if a matching network was found; otherwise <see langword="false"/>.</returns>
    public bool TryGetNetwork(string bin, out IssuingNetwork network)
    {
        if (string.IsNullOrWhiteSpace(bin))
        {
            network = IssuingNetwork.Unknown;
            return false;
        }

        return this.lookup.TryGetValue(bin, out network);
    }

    private void AddPrefix(string prefix, IssuingNetwork network)
    {
        this.lookup.Add(prefix, network);
    }

    private void AddRange(int start, int end, IssuingNetwork network)
    {
        for (int i = start; i <= end; i++)
        {
            this.AddPrefix(i.ToString(), network);
        }
    }

    private void LoadVisa()
    {
        this.AddPrefix("4", IssuingNetwork.Visa);
    }

    private void LoadMasterCard()
    {
        // Old Mastercard ranges: 51–55
        this.AddPrefix("51", IssuingNetwork.MasterCard);
        this.AddPrefix("52", IssuingNetwork.MasterCard);
        this.AddPrefix("53", IssuingNetwork.MasterCard);
        this.AddPrefix("54", IssuingNetwork.MasterCard);
        this.AddPrefix("55", IssuingNetwork.MasterCard);

        // New Mastercard 2-series: 2221–2720
        this.AddRange(2221, 2720, IssuingNetwork.MasterCard);
    }

    private void LoadAmericanExpress()
    {
        this.AddPrefix("34", IssuingNetwork.AmericanExpress);
        this.AddPrefix("37", IssuingNetwork.AmericanExpress);
    }

    private void LoadDiscover()
    {
        this.AddPrefix("6011", IssuingNetwork.Discover);
        this.AddPrefix("65", IssuingNetwork.Discover);
        this.AddRange(644, 649, IssuingNetwork.Discover);
        this.AddRange(622126, 622925, IssuingNetwork.Discover);
    }

    private void LoadDinersClub()
    {
        this.AddRange(300, 305, IssuingNetwork.DinersClub);
        this.AddPrefix("36", IssuingNetwork.DinersClub);
        this.AddPrefix("38", IssuingNetwork.DinersClub);
        this.AddPrefix("39", IssuingNetwork.DinersClub);
    }

    private void LoadJcb()
    {
        this.AddRange(3528, 3589, IssuingNetwork.Jcb);
    }

    private void LoadUnionPay()
    {
        this.AddPrefix("62", IssuingNetwork.UnionPay);
    }
}
