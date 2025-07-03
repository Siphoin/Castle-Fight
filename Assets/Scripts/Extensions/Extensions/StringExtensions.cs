namespace CastleFight.Extensions
{
    public static class StringExtensions
    {
        public static string ToGameCurrency(this double value)
        {
            string[] suffixes = { "", "K", "M", "B", "T", "Q" };
            int suffixIndex = 0;

            while (value >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                value /= 1000;
                suffixIndex++;
            }

            string format;
            if (suffixIndex == 0)
            {
                format = "0";
            }
            else if (value < 10)
            {
                format = "0.0";
            }
            else
            {
                format = "0";
            }

            return value.ToString(format) + suffixes[suffixIndex];
        }
        public static string ToGameCurrency(this int value)
        {
            return ((double)value).ToGameCurrency();
        }

        public static string ToGameCurrency(this uint value)
        {
            return ((double)value).ToGameCurrency();
        }
    }
}
