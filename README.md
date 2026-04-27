# DigitPrefixLookup

A trie-based lookup that maps digit-only prefixes to values, housed in the `Collections.Specialized` namespace.

## Overview

`DigitPrefixLookup<TValue>` stores prefix/value pairs where each key is a string of decimal digits. A lookup walk returns the value for the longest matching prefix, so more-specific (longer) prefixes take priority over shorter ones that share the same leading digits.

A common use case is identifying a payment card issuer from its **Bank Identification Number (BIN)**, the leading 6–8 digits of a card number. For example, the BINs `51`–`55` all map to MasterCard, so a card number beginning with `521853` is identified as MasterCard via the `52` prefix.

| Issuer     | BIN prefix(es)                               |
|------------|----------------------------------------------|
| Visa       | `4`                                          |
| MasterCard | `51`–`55`, `2221`–`2720`                     |
| Amex       | `34`, `37`                                   |
| Discover   | `6011`, `622126`–`622925`, `644`–`649`, `65` |
| Diners     | `300`–`305`, `36`, `38`, `39`                |
| JCB        | `3528`–`3589`                                |
| UnionPay   | `62` (excluding the Discover sub-range)      |

## API

```csharp
var lookup = new DigitPrefixLookup<string>();

// Add prefix → value pairs
lookup.Add("51", "MasterCard");
lookup.Add("52", "MasterCard");
lookup.TryAdd("4",  "Visa");          // returns false if prefix already exists

// Look up by any card number (returns the longest matching registered prefix)
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
- Lookup is **longest-match**: given a card number `6221260000000000` and registered prefixes `62` and `622126`, the value for the longer prefix `622126` is returned.

## BinNetworkLookup

The `Payments` library provides a ready-to-use `BinNetworkLookup` that wraps `DigitPrefixLookup` with all major network BIN ranges pre-loaded.

```csharp
var lookup = BinNetworkLookup.Instance;

if (lookup.TryGetNetwork("4111111111111111", out IssuingNetwork network))
    Console.WriteLine(network);   // Visa
```

`TryGetNetwork` accepts any string of digits (a full card number works; only the leading digits matter) and returns the matched `IssuingNetwork` enum value, or `IssuingNetwork.Unknown` if no prefix matches.

## Project structure

```text
Collections.Specialized/      # trie library (net10.0)
  DigitPrefixLookup.cs        # trie implementation
  IPrefixLookup.cs            # read/write interface
  IReadOnlyPrefixLookup.cs    # read-only view interface
  PrefixNotFoundException.cs  # exception for missing prefixes

Payments/                     # card-network library (net10.0)
  CardNetworks/
    BinNetworkLookup.cs       # pre-loaded BIN → IssuingNetwork lookup
    IssuingNetwork.cs         # enum of supported card networks

DigitPrefixLookupTest/        # xUnit tests for the trie (net10.0, FluentAssertions)
PaymentsTest/                 # xUnit tests for BinNetworkLookup (net10.0, FluentAssertions)
```

## Further reading

- [BIN list by issuer](https://www.bincodes.com/bin-list/)
- [BIN ranges for major card networks](https://neapay.com/post/bin-list-range-for-mastercard-visa-amex-diners-discover-jcb-cup_94.html)
