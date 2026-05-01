namespace Payments.CardValidation;

/// <summary>
/// Validates a card number digit sequence using the Luhn algorithm.
/// </summary>
public static class LuhnValidator
{
    /// <summary>
    /// Determines whether the given digit sequence passes the Luhn check.
    /// </summary>
    /// <param name="value">The digit sequence to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the sequence is non-empty, contains only digits,
    /// and the Luhn checksum is divisible by 10; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsValid(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
        {
            return false;
        }

        int sum = 0;
        bool doubleDigit = false;

        for (int i = value.Length - 1; i >= 0; i--)
        {
            char c = value[i];

            if (c is < '0' or > '9')
            {
                return false;
            }

            int digit = c - '0';

            if (doubleDigit)
            {
                digit *= 2;

                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            doubleDigit = !doubleDigit;
        }

        return sum % 10 == 0;
    }
}
