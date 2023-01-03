using System.Security.Cryptography;

namespace InventorySystem.Data
{
    internal class Utils
    {

        private const char _segmentDelimiter = ':';

        public static string HashSecret(string input)
        {
            var saltSize = 16;
            var iterations = 100_000;
            var keySize = 32;
            HashAlgorithmName algorithm = HashAlgorithmName.SHA256;
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

            return string.Join(
                _segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                iterations,
                algorithm
            );
        }

        public static bool VerifyHash(string input, string hashString)
        {
            string[] segments = hashString.Split(_segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }

        public static string GetJSONfilePath()
        {
            return @"C:\Users\Subriti\C#_Coursework\";
        }

        public static string GetAppUsersFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "users.json");
        }

        public static string GetItemsFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "items.json");
        }

        public static string GetInventoryLogFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "inventoryLog.json");
        }

        public static string GetAdminLoginFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "adminLogin.json");
        }
    }
}
