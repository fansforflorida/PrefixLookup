namespace DigitPrefixLookupTest;

using Collections.Specialized;
using FluentAssertions;

public class DigitPrefixLookupTests
{
    [Fact]
    public void Count_EmptyLookup_IsZero()
    {
        DigitPrefixLookup<string> lookup = [];

        lookup.Count.Should().Be(0);
    }

    [Fact]
    public void Count_AfterOneAdd_IsOne()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        lookup.Count.Should().Be(1);
    }

    [Fact]
    public void TryGetValue_WithExistingPrefix_ReturnsTrue()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var result = lookup.TryGetValue("52", out var value);
        result.Should().BeTrue();
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void TryAdd_DuplicatePrefix_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var result = lookup.TryAdd("52", "MasterCard2");
        result.Should().BeFalse();
    }

    [Fact]
    public void Add_DuplicatePrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        lookup.Invoking(l => l.Add("52", "MasterCard2"))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryAdd_NullPrefix_ThrowsArgumentNullException()
    {
        DigitPrefixLookup<string> lookup = [];

        lookup.Invoking(l => l.TryAdd(null!, "MasterCard"))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Add_NullPrefix_ThrowsArgumentNullException()
    {
        DigitPrefixLookup<string> lookup = [];

        lookup.Invoking(l => l.Add(null!, "MasterCard"))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryAdd_NonDigitPrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = [];

        lookup.Invoking(l => l.TryAdd("5A", "MasterCard"))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Add_NonDigitPrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = [];

        lookup.Invoking(l => l.Add("5A", "MasterCard"))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryGetValue_PrefixNotFound_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var result = lookup.TryGetValue("99", out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_InputStartsWithRegisteredPrefix_ReturnsTrueWithValue()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var result = lookup.TryGetValue("521853", out var value);
        result.Should().BeTrue();
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void TryGetValue_NonDigitPrefix_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var result = lookup.TryGetValue("5A", out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_ShorterPrefixThanRegistered_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var result = lookup.TryGetValue("5", out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_OverlappingPrefixes_ReturnsLongestMatch()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "62", "UnionPay" },
            { "622126", "Discover" },
        };

        var result = lookup.TryGetValue("6221260000000000", out var value);
        result.Should().BeTrue();
        value.Should().Be("Discover");
    }

    [Fact]
    public void TryGetValue_OverlappingPrefixes_WhenOnlyShortPrefixMatches_ReturnsShortMatch()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "62", "UnionPay" },
            { "622126", "Discover" },
        };

        var result = lookup.TryGetValue("6250000000000000", out var value);
        result.Should().BeTrue();
        value.Should().Be("UnionPay");
    }

    [Fact]
    public void Indexer_Set_AddsPrefix()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            ["52"] = "MasterCard"
        };

        lookup.Count.Should().Be(1);
        lookup["52"].Should().Be("MasterCard");
    }

    [Fact]
    public void Indexer_Get_WithExistingPrefix_ReturnsValue()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        var value = lookup["52"];
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void Indexer_Get_WithNonexistentPrefix_ThrowsPrefixNotFoundException()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "52", "MasterCard" }
        };

        lookup.Invoking(l => _ = l["99"])
            .Should().Throw<PrefixNotFoundException>();
    }

    [Fact]
    public void Indexer_Set_WithDuplicatePrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            ["52"] = "MasterCard"
        };

        lookup.Invoking(l => l["52"] = "MasterCard2")
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Indexer_Get_WithInputStartingWithRegisteredPrefix_ReturnsValue()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            ["52"] = "MasterCard"
        };

        var value = lookup["521853"];
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void GetEnumerator_WithMultiplePrefixes_ReturnsInSortedOrder()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "51", "MasterCard" },
            { "4", "Visa" },
            { "34", "Amex" },
            { "6011", "Discover" },
            { "2221", "MasterCard" },
        };

        var result = lookup
            .Select(kvp => new { kvp.Key, kvp.Value })
            .ToList();

        result.Should().BeEquivalentTo(
            [
                new { Key ="2221", Value = "MasterCard" },
                new { Key ="34",   Value = "Amex" },
                new { Key ="4",    Value = "Visa" },
                new { Key ="51",   Value = "MasterCard" },
                new { Key ="6011", Value = "Discover" },
            ],
            options => options.WithStrictOrdering());
    }

    [Fact]
    public void Keys_WithMultiplePrefixes_ReturnsInSortedOrder()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "51", "MasterCard" },
            { "4", "Visa" },
            { "34", "Amex" },
            { "6011", "Discover" },
            { "2221", "MasterCard" },
        };

        string[] expected = ["2221", "34", "4", "51", "6011"];

        lookup.Keys.Should().Equal(expected);
    }

    [Fact]
    public void Values_WithMultiplePrefixes_ReturnsInSortedOrder()
    {
        DigitPrefixLookup<string> lookup = new()
        {
            { "51", "MasterCard" },
            { "4", "Visa" },
            { "34", "Amex" },
            { "6011", "Discover" },
            { "2221", "MasterCard" },
        };

        var values = lookup.Values.ToList();

        values.Should().ContainInOrder("MasterCard", "Amex", "Visa", "MasterCard", "Discover");
    }
}
