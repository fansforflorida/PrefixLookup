namespace DigitPrefixLookupTest;

using Collections.Specialized;
using FluentAssertions;

public class DigitPrefixLookupTests
{
    [Fact]
    public void Count_EmptyLookup_IsZero()
    {
        DigitPrefixLookup<string> lookup = new();

        lookup.Count.Should().Be(0);
    }

    [Fact]
    public void Count_AfterOneAdd_IsOne()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        lookup.Count.Should().Be(1);
    }

    [Fact]
    public void TryGetValue_WithExistingPrefix_ReturnsTrue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var result = lookup.TryGetValue("52", out var value);
        result.Should().BeTrue();
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void TryAdd_DuplicatePrefix_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var result = lookup.TryAdd("52", "MasterCard2");
        result.Should().BeFalse();
    }

    [Fact]
    public void Add_DuplicatePrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        lookup.Invoking(l => l.Add("52", "MasterCard2"))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryAdd_NullPrefix_ThrowsArgumentNullException()
    {
        DigitPrefixLookup<string> lookup = new();

        lookup.Invoking(l => l.TryAdd(null!, "MasterCard"))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Add_NullPrefix_ThrowsArgumentNullException()
    {
        DigitPrefixLookup<string> lookup = new();

        lookup.Invoking(l => l.Add(null!, "MasterCard"))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryAdd_NonDigitPrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = new();

        lookup.Invoking(l => l.TryAdd("5A", "MasterCard"))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Add_NonDigitPrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = new();

        lookup.Invoking(l => l.Add("5A", "MasterCard"))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryGetValue_PrefixNotFound_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var result = lookup.TryGetValue("99", out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_InputStartsWithRegisteredPrefix_ReturnsTrueWithValue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var result = lookup.TryGetValue("521853", out var value);
        result.Should().BeTrue();
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void TryGetValue_NonDigitPrefix_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var result = lookup.TryGetValue("5A", out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_ShorterPrefixThanRegistered_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var result = lookup.TryGetValue("5", out _);
        result.Should().BeFalse();
    }

    [Fact]
    public void Indexer_Set_AddsPrefix()
    {
        DigitPrefixLookup<string> lookup = new();

        lookup["52"] = "MasterCard";

        lookup.Count.Should().Be(1);
        lookup["52"].Should().Be("MasterCard");
    }

    [Fact]
    public void Indexer_Get_WithExistingPrefix_ReturnsValue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var value = lookup["52"];
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void Indexer_Get_WithNonexistentPrefix_ThrowsPrefixNotFoundException()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        lookup.Invoking(l => _ = l["99"])
            .Should().Throw<PrefixNotFoundException>();
    }

    [Fact]
    public void Indexer_Set_WithDuplicatePrefix_ThrowsArgumentException()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup["52"] = "MasterCard";

        lookup.Invoking(l => l["52"] = "MasterCard2")
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Indexer_Get_WithInputStartingWithRegisteredPrefix_ReturnsValue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup["52"] = "MasterCard";

        var value = lookup["521853"];
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void AsReadOnly_ReturnsIReadOnlyPrefixLookup()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        readOnly.Should().BeAssignableTo<IReadOnlyPrefixLookup<IEnumerable<char>, string>>();
    }

    [Fact]
    public void AsReadOnly_Count_ReflectsOriginalLookupCount()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");
        lookup.Add("4", "Visa");

        var readOnly = lookup.AsReadOnly();
        readOnly.Count.Should().Be(2);
    }

    [Fact]
    public void AsReadOnly_TryGetValue_WithExistingPrefix_ReturnsTrue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        var result = readOnly.TryGetValue("52", out var value);

        result.Should().BeTrue();
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void AsReadOnly_TryGetValue_WithNonexistentPrefix_ReturnsFalse()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        var result = readOnly.TryGetValue("99", out _);

        result.Should().BeFalse();
    }

    [Fact]
    public void AsReadOnly_TryGetValue_WithInputStartingWithRegisteredPrefix_ReturnsTrueWithValue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        var result = readOnly.TryGetValue("521853", out var value);

        result.Should().BeTrue();
        value.Should().Be("MasterCard");
    }

    [Fact]
    public void AsReadOnly_Indexer_Get_WithExistingPrefix_ReturnsValue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        var value = readOnly["52"];

        value.Should().Be("MasterCard");
    }

    [Fact]
    public void AsReadOnly_Indexer_Get_WithNonexistentPrefix_ThrowsPrefixNotFoundException()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        readOnly.Invoking(r => _ = r["99"])
            .Should().Throw<PrefixNotFoundException>();
    }

    [Fact]
    public void AsReadOnly_Indexer_Get_WithInputStartingWithRegisteredPrefix_ReturnsValue()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        var value = readOnly["521853"];

        value.Should().Be("MasterCard");
    }

    [Fact]
    public void AsReadOnly_ReflectsChangesToOriginalLookup()
    {
        DigitPrefixLookup<string> lookup = new();
        lookup.Add("52", "MasterCard");

        var readOnly = lookup.AsReadOnly();
        readOnly.Count.Should().Be(1);

        lookup.Add("4", "Visa");

        readOnly.Count.Should().Be(2);
        var result = readOnly.TryGetValue("4", out var value);
        result.Should().BeTrue();
        value.Should().Be("Visa");
    }

    [Fact]
    public void AsReadOnly_MultipleCallsReturnDifferentInstances()
    {
        DigitPrefixLookup<string> lookup = new();

        var readOnly1 = lookup.AsReadOnly();
        var readOnly2 = lookup.AsReadOnly();

        readOnly1.Should().NotBeSameAs(readOnly2);
    }
}
