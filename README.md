# DigitPrefixLookup

A trie-based lookup that maps digit-only prefixes to values, housed in the `Collections.Specialized` namespace.

## Overview

`DigitPrefixLookup<TValue>` stores prefix/value pairs where each key is a string of decimal digits. A lookup walk stops at the first matching prefix, so shorter prefixes take priority over longer ones that share the same leading digits.

A common use case is identifying a payment card issuer from its **Bank Identification Number (BIN)**, the leading 6–8 digits of a card number. For example, the BINs `51`–`55` all map to MasterCard, so a card number beginning with `521853` is identified as MasterCard via the `52` prefix.

| Issuer     | BIN prefix(es)                               |
|------------|----------------------------------------------|
| Visa       | `4`                                          |
| MasterCard | `51`–`55`, `2221`–`2720`                     |
| Amex       | `34`, `37`                                   |
| Discover   | `6011`, `622126`–`622925`, `644`–`649`, `65` |
| Diners     | `300`–`305`, `36`, `38`                      |
| JCB        | `3528`–`3589`                                |

## API

```csharp
var lookup = new DigitPrefixLookup<string>();

// Add prefix → value pairs
lookup.Add("51", "MasterCard");
lookup.Add("52", "MasterCard");
lookup.TryAdd("4",  "Visa");          // returns false if prefix already exists

// Look up by any card number (match stops at the first registered prefix)
if (lookup.TryGetValue("521853", out var issuer))
    Console.WriteLine(issuer);        // MasterCard

// Indexer throws PrefixNotFoundException on miss
string brand = lookup["4111111111111111"];

// Restrict to a read-only view (direct assignment, no wrapper needed)
IReadOnlyPrefixLookup<string, string> ro = lookup;

Console.WriteLine(lookup.Count);      // number of registered prefixes
```

### Interface hierarchy

`IPrefixLookup<TKey, TValue>` extends `IReadOnlyPrefixLookup<TKey, TValue>`, which in turn extends `IReadOnlyCollection<KeyValuePair<TKey, TValue>>`. This means any `DigitPrefixLookup<TValue>` can be assigned directly to `IReadOnlyPrefixLookup` without a wrapper.

`IReadOnlyPrefixLookup` represents a **read-only view through that reference**. It does not guarantee that the underlying collection is immutable. Code holding an `IPrefixLookup` reference to the same instance may still add entries. This mirrors the convention used in the .NET BCL (e.g. `List<T>` implements `IReadOnlyList<T>`, yet the list itself remains mutable).

### Key behaviours

- Prefixes must contain only ASCII digits (`0`–`9`); non-digit input throws `ArgumentException`.
- Adding a duplicate prefix throws `ArgumentException` (`Add`) or returns `false` (`TryAdd`).
- Reading a missing prefix throws `PrefixNotFoundException` (indexer) or returns `false` (`TryGetValue`).
- Lookup is **prefix-first**: given a card number `521853` and a registered prefix `52`, the value is returned after consuming just the first two digits.

## Project structure

```text
Collections.Specialized/      # library (net10.0)
  DigitPrefixLookup.cs        # trie implementation
  IPrefixLookup.cs            # read/write interface
  IReadOnlyPrefixLookup.cs    # read-only view interface
  PrefixNotFoundException.cs  # exception for missing prefixes

DigitPrefixLookupTest/        # xUnit test suite (net10.0, FluentAssertions)
```

## Further reading

- [BIN list by issuer](https://www.bincodes.com/bin-list/)
- [BIN ranges for major card networks](https://neapay.com/post/bin-list-range-for-mastercard-visa-amex-diners-discover-jcb-cup_94.html)
