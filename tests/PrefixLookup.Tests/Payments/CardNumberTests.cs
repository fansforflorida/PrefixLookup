namespace PaymentsTest;

using FluentAssertions;
using Payments.CardNetworks;
using Payments.CardValidation;

public class CardNumberTests
{
    [Theory]
    [InlineData("4242424242424242", IssuingNetwork.Visa)]
    [InlineData("5500005555555559", IssuingNetwork.MasterCard)]
    [InlineData("371449635398431",  IssuingNetwork.AmericanExpress)]
    public void Network_ReturnsExpectedNetwork(string value, IssuingNetwork expected)
    {
        new CardNumber(value).Network.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366")]
    [InlineData("4242424242424242")]
    [InlineData("5500005555555559")]
    [InlineData("371449635398431")]
    public void IsLuhnValid_WithValidCardNumber_ReturnsTrue(string value)
    {
        new CardNumber(value).IsLuhnValid.Should().BeTrue();
    }

    [Fact]
    public void IsLuhnValid_WithInvalidCardNumber_ReturnsFalse()
    {
        new CardNumber("4532015112830367").IsLuhnValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("4532015112830366", true)]   // Visa 16-digit
    [InlineData("5500005555555559", true)]   // MasterCard 16-digit
    [InlineData("371449635398431",  true)]   // Amex 15-digit
    [InlineData("36000000000000",   true)]   // Diners Club 14-digit
    [InlineData("4111",             false)]  // Visa prefix but too short
    public void IsValidLength_ReturnsExpected(string value, bool expected)
    {
        new CardNumber(value).IsValidLength.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366", "************0366")]
    [InlineData("4242",             "****")]
    [InlineData("123",              "***")]
    public void Masked_ReturnsLastFourWithLeadingAsterisks(string value, string expected)
    {
        new CardNumber(value).Masked.Should().Be(expected);
    }

    [Fact]
    public void ToString_ReturnsRawValue()
    {
        const string value = "4532015112830366";
        new CardNumber(value).ToString().Should().Be(value);
    }
}
