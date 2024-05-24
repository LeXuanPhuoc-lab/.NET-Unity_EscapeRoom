using EscapeRoomAPI.Enums;

namespace EscapeRoomAPI.Utils
{
    public static class NumberUtils
    {
        public static bool IsAscending(int number)
        {
            int lastDigit = number % 10;
            number /= 10;

            while (number > 0)
            {
                int digit = number % 10;
                if (digit > lastDigit)
                    return false;
                lastDigit = digit;
                number /= 10;
            }

            return true;
        }

        public static bool IsDescending(int number)
        {
            int lastDigit = number % 10;
            number /= 10;

            while (number > 0)
            {
                int digit = number % 10;
                if (digit < lastDigit)
                    return false;
                lastDigit = digit;
                number /= 10;
            }

            return true;
        }

        public static int CombineDigitsIntoNumber(List<int> digits, string orderBy)
        {
            var ascendingOrder = orderBy.Equals(nameof(UnclockHint.Ascending));
            var sortedDigits = ascendingOrder ? digits.OrderBy(d => d).ToList() : digits.OrderByDescending(d => d).ToList();
            string combinedString = string.Join("", sortedDigits);
            return int.Parse(combinedString);
        }
    }
}
