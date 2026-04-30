namespace Payments.CardValidation;

/// <summary>
/// Validates a Primary Account Number (PAN) using the Luhn algorithm.
/// </summary>
public static class LuhnValidator
{
    /// <summary>
    /// Determines whether the given digit sequence passes the Luhn check.
    /// </summary>
    /// <param name="pan">The digit sequence to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the sequence is non-empty, contains only digits,
    /// and the Luhn checksum is divisible by 10; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsValid(ReadOnlySpan<char> pan)
    {
        if (pan.IsEmpty)
        {
            return false;
        }

        int sum = 0;
        bool doubleDigit = false;

        for (int i = pan.Length - 1; i >= 0; i--)
        {
            char c = pan[i];

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
