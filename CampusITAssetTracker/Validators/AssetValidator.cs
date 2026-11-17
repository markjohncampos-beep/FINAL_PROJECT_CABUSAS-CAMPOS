
using System;

namespace CampusITAssetTracker.Validators
{
    public static class AssetValidator
    {
        public static bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            string[] parts = ip.Split('.');

            if (parts.Length != 4)
                return false;

            foreach (string part in parts)
            {
                if (part.Length < 1 || part.Length > 3)
                    return false;

                if (!byte.TryParse(part, out _))
                    return false;
            }

            return true;
        }

        public static bool IsValidMacAddress(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
                return false;

            string[] parts = mac.Split(':');

            if (parts.Length != 6)
                return false;

            foreach (string part in parts)
            {
                if (part.Length != 2 ||
                    !Uri.IsHexDigit(part[0]) ||
                    !Uri.IsHexDigit(part[1]))
                {
                    return false;
                }
            }

            return true;
        }
        public static bool IsValidAssetTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return System.Text.RegularExpressions.Regex
                .IsMatch(tag, @"^[A-Z]{2}-\d{3}$");
        }

        public static bool IsValidBorrowerId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            return System.Text.RegularExpressions.Regex
                .IsMatch(id, @"^\d{4}-\d{5}$");
        }
    }
}
