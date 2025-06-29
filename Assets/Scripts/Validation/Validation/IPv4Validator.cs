using System.Net;
using System.Text.RegularExpressions;

namespace CastleFight.Validation
{
    public static class IPValidator
    {
        private static readonly Regex IPv4Regex = new Regex(
            @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            RegexOptions.Compiled);

        public static bool IsValidIPv4(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return false;
            }

            if (!IPv4Regex.IsMatch(ipAddress))
            {
                return false;
            }
            return IPAddress.TryParse(ipAddress, out _);
        }

    }
}
