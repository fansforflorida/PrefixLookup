namespace PaymentsTest;

using FluentAssertions;
using Payments;

public class CardExpiryTests
{
    [Theory]
    [InlineData("0428",  4, 2028)]
    [InlineData("04/28", 4, 2028)]
    [InlineData("04-28", 4, 2028)]
    [InlineData("1299", 12, 2099)]
    [InlineData("0100",  1, 2000)]
    public void TryParse_WithValidInput_ParsesMonthAndYear(string input, int expectedMonth, int expectedYear)
    {
        var success = CardExpiry.TryParse(input, out var expiry);

        success.Should().BeTrue();
        expiry.Month.Should().Be(expectedMonth);
        expiry.Year.Should().Be(expectedYear);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("0028")]   // month 00 is invalid
    [InlineData("1328")]   // month 13 is invalid
    [InlineData("ab28")]   // non-digit month
    [InlineData("04ab")]   // non-digit year
    [InlineData("428")]    // too short
    [InlineData("042800")] // too long
    public void TryParse_WithInvalidInput_ReturnsFalse(string? input)
    {
        CardExpiry.TryParse(input, out _).Should().BeFalse();
    }

    [Theory]
    [InlineData(2026, 4,  2026, 5,  1,  true)]  // first day of following month → expired
    [InlineData(2026, 4,  2026, 4,  30, false)] // last day of expiry month → not expired
    [InlineData(2026, 4,  2026, 4,  1,  false)] // first day of expiry month → not expired
    [InlineData(2026, 4,  2026, 3,  31, false)] // day before expiry month → not expired
    [InlineData(2025, 12, 2026, 1,  1,  true)]  // year boundary → expired
    public void IsExpiredAsOf_ReturnsExpected(
        int expiryYear, int expiryMonth,
        int asOfYear, int asOfMonth, int asOfDay,
        bool expected)
    {
        var expiry = new CardExpiry(expiryMonth, expiryYear);
        var asOf = new DateOnly(asOfYear, asOfMonth, asOfDay);

        expiry.IsExpiredAsOf(asOf).Should().Be(expected);
    }

    [Fact]
    public void IsExpired_WithFarFutureExpiry_ReturnsFalse()
    {
        new CardExpiry(12, 2099).IsExpired.Should().BeFalse();
    }

    [Theory]
    [InlineData(4,  2028, "04/28")]
    [InlineData(12, 2099, "12/99")]
    [InlineData(1,  2000, "01/00")]
    public void ToString_ReturnsMMYYFormat(int month, int year, string expected)
    {
        new CardExpiry(month, year).ToString().Should().Be(expected);
    }
}
