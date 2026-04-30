namespace PaymentsTest;

using FluentAssertions;
using Payments.CardNetworks;
using Payments.CardValidation;

public class PanTests
{
    [Theory]
    [InlineData("4242424242424242", IssuingNetwork.Visa)]
    [InlineData("5500005555555559", IssuingNetwork.MasterCard)]
    [InlineData("371449635398431",  IssuingNetwork.AmericanExpress)]
    public void Network_ReturnsExpectedNetwork(string value, IssuingNetwork expected)
    {
        new Pan(value).Network.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366")]
    [InlineData("4242424242424242")]
    [InlineData("5500005555555559")]
    [InlineData("371449635398431")]
    public void IsLuhnValid_WithValidPan_ReturnsTrue(string value)
    {
        new Pan(value).IsLuhnValid.Should().BeTrue();
    }

    [Fact]
    public void IsLuhnValid_WithInvalidPan_ReturnsFalse()
    {
        new Pan("4532015112830367").IsLuhnValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("4532015112830366", true)]   // Visa 16-digit
    [InlineData("5500005555555559", true)]   // MasterCard 16-digit
    [InlineData("371449635398431",  true)]   // Amex 15-digit
    [InlineData("36000000000000",   true)]   // Diners Club 14-digit
    [InlineData("4111",             false)]  // Visa prefix but too short
    public void IsValidLength_ReturnsExpected(string value, bool expected)
    {
        new Pan(value).IsValidLength.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532015112830366", "************0366")]
    [InlineData("4242",             "****")]
    [InlineData("123",              "***")]
    public void Masked_ReturnsLastFourWithLeadingAsterisks(string value, string expected)
    {
        new Pan(value).Masked.Should().Be(expected);
    }

    [Fact]
    public void ToString_ReturnsRawValue()
    {
        const string value = "4532015112830366";
        new Pan(value).ToString().Should().Be(value);
    }
}
