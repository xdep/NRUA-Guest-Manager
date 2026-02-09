using System;
using System.Security.Cryptography;
using System.Text;

namespace NRUAGuestManager
{
    public static class LicenseValidator
    {
        // Charset: A-Z plus 2-9 (excludes 0, 1, O, I to avoid ambiguity)
        private const string Charset = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private static readonly byte[] HmacSecret = Encoding.UTF8.GetBytes("NRUAGuestMgr-2024-RD1312-LicKey!");

        public static bool IsValidKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            key = key.Trim().ToUpperInvariant();

            // Check format: NRUA-XXXX-XXXX-XXXX-XXXX
            var parts = key.Split('-');
            if (parts.Length != 5)
                return false;

            if (parts[0] != "NRUA")
                return false;

            // Check each group is exactly 4 chars from the charset
            for (int i = 1; i < 5; i++)
            {
                if (parts[i].Length != 4)
                    return false;
                foreach (char c in parts[i])
                {
                    if (Charset.IndexOf(c) < 0)
                        return false;
                }
            }

            // Verify checksum: DDDD must match HMAC of AAAA-BBBB-CCCC
            string input = $"{parts[1]}-{parts[2]}-{parts[3]}";
            string expectedCheck = ComputeCheckGroup(input);

            return parts[4] == expectedCheck;
        }

        public static string ComputeCheckGroup(string input)
        {
            using var hmac = new HMACSHA256(HmacSecret);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sb = new StringBuilder(4);
            for (int i = 0; i < 4; i++)
            {
                sb.Append(Charset[hash[i] % Charset.Length]);
            }
            return sb.ToString();
        }

        public static string GenerateKey()
        {
            var rng = RandomNumberGenerator.Create();
            var groups = new string[3];

            for (int g = 0; g < 3; g++)
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var sb = new StringBuilder(4);
                for (int i = 0; i < 4; i++)
                {
                    sb.Append(Charset[bytes[i] % Charset.Length]);
                }
                groups[g] = sb.ToString();
            }

            string input = $"{groups[0]}-{groups[1]}-{groups[2]}";
            string check = ComputeCheckGroup(input);

            return $"NRUA-{groups[0]}-{groups[1]}-{groups[2]}-{check}";
        }

        public static string GetLicenseFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return System.IO.Path.Combine(appData, "NRUAGuestManager", "license.key");
        }

        public static string ReadStoredKey()
        {
            string path = GetLicenseFilePath();
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllText(path).Trim();
            }
            return null;
        }

        public static void SaveKey(string key)
        {
            string path = GetLicenseFilePath();
            string dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            System.IO.File.WriteAllText(path, key.Trim().ToUpperInvariant());
        }
    }
}
