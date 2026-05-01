namespace PaymentsTest;

using FluentAssertions;
using Payments.CardValidation;

public class LuhnValidatorTests
{
    [Theory]
    [InlineData("4532015112830366")]  // Visa
    [InlineData("4242424242424242")]  // Visa
    [InlineData("5500005555555559")]  // MasterCard
    [InlineData("371449635398431")]   // American Express
    [InlineData("79927398713")]       // Canonical Luhn test vector
    public void IsValid_WithValidCardNumber_ReturnsTrue(string value)
    {
        LuhnValidator.IsValid(value).Should().BeTrue();
    }

    [Theory]
    [InlineData("4532015112830367")]  // Valid Visa with last digit incremented
    [InlineData("79927398714")]       // Canonical test vector with last digit incremented
    [InlineData("1234567890123456")]  // Arbitrary invalid sequence
    public void IsValid_WithInvalidChecksum_ReturnsFalse(string value)
    {
        LuhnValidator.IsValid(value).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("123abc")]
    [InlineData("4111-1111-1111-1111")]
    public void IsValid_WithNonDigitOrEmptyInput_ReturnsFalse(string value)
    {
        LuhnValidator.IsValid(value).Should().BeFalse();
    }
}
