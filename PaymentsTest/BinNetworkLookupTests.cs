namespace PaymentsTest;

using FluentAssertions;
using Payments.CardNetworks;

public class BinNetworkLookupTests
{
    private readonly BinNetworkLookup sut = BinNetworkLookup.Instance;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryGetNetwork_WithBlankBin_ReturnsFalseAndUnknown(string? bin)
    {
        var result = sut.TryGetNetwork(bin!, out var network);

        result.Should().BeFalse();
        network.Should().Be(IssuingNetwork.Unknown);
    }

    [Fact]
    public void TryGetNetwork_WithUnrecognizedBin_ReturnsFalseAndUnknown()
    {
        var result = sut.TryGetNetwork("9999999", out var network);

        result.Should().BeFalse();
        network.Should().Be(IssuingNetwork.Unknown);
    }

    // Visa: prefix 4
    [Theory]
    [InlineData("4")]
    [InlineData("4111111111111111")]
    [InlineData("4242424242424242")]
    public void TryGetNetwork_WithVisaBin_ReturnsVisa(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.Visa);
    }

    // Mastercard: classic 51-55, new 2-series 2221-2720
    [Theory]
    [InlineData("5100000000000000")]
    [InlineData("5200000000000000")]
    [InlineData("5300000000000000")]
    [InlineData("5400000000000000")]
    [InlineData("5500000000000000")]
    [InlineData("2221")]
    [InlineData("2720")]
    [InlineData("2500")]
    public void TryGetNetwork_WithMasterCardBin_ReturnsMasterCard(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.MasterCard);
    }

    [Theory]
    [InlineData("2220")]  // just below MasterCard 2-series
    [InlineData("2721")]  // just above MasterCard 2-series
    [InlineData("3527")]  // just below JCB range
    [InlineData("3590")]  // just above JCB range
    public void TryGetNetwork_WithOutOfRangeBin_ReturnsNotFound(string bin)
    {
        var result = sut.TryGetNetwork(bin, out _);

        result.Should().BeFalse();
    }

    // American Express: 34, 37
    [Theory]
    [InlineData("340000000000000")]
    [InlineData("370000000000000")]
    public void TryGetNetwork_WithAmexBin_ReturnsAmericanExpress(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.AmericanExpress);
    }

    // Discover: 6011, 622126-622925, 644-649, 65
    [Theory]
    [InlineData("6011000000000000")]
    [InlineData("6221260000000000")]
    [InlineData("6229250000000000")]
    [InlineData("6500000000000000")]
    [InlineData("650")]
    [InlineData("644")]
    [InlineData("649")]
    public void TryGetNetwork_WithDiscoverBin_ReturnsDiscover(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.Discover);
    }

    [Theory]
    [InlineData("643")]
    [InlineData("622125")]
    [InlineData("622926")]
    public void TryGetNetwork_WithDiscoverRangesOutOfBounds_DoesNotReturnDiscover(string bin)
    {
        sut.TryGetNetwork(bin, out var network);

        network.Should().NotBe(IssuingNetwork.Discover);
    }

    // Diners Club: 300-305, 36, 38, 39
    [Theory]
    [InlineData("300")]
    [InlineData("305")]
    [InlineData("302")]
    [InlineData("36000000000000")]
    [InlineData("38000000000000")]
    [InlineData("39000000000000")]
    public void TryGetNetwork_WithDinersClubBin_ReturnsDinersClub(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.DinersClub);
    }

    // JCB: 3528-3589
    [Theory]
    [InlineData("3528")]
    [InlineData("3589")]
    [InlineData("3550")]
    public void TryGetNetwork_WithJcbBin_ReturnsJcb(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.Jcb);
    }

    // UnionPay: 62 (excluding the Discover sub-range 622126-622925)
    [Theory]
    [InlineData("6200000000000000")]
    [InlineData("6250000000000000")]
    public void TryGetNetwork_WithUnionPayBin_ReturnsUnionPay(string bin)
    {
        var result = sut.TryGetNetwork(bin, out var network);

        result.Should().BeTrue();
        network.Should().Be(IssuingNetwork.UnionPay);
    }
}
