using System.Security.Cryptography;

namespace InventorySystem.Data
{
    internal class Utils
    {

        private const char _segmentDelimiter = ':';

        //Hashing the password by adding salt
        public static string HashPassword(string input)
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

        //verifying if two entered values have the same hash; the entered password during login and the one stored in the JSON file
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

        //specifying the location to store the JSON folder
        public static string GetJSONfilePath()
        {
            return @"C:\Users\Subriti\C#_Coursework\";
        }

        //specifying the name and location for storing User data
        public static string GetAppUsersFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "users.json");
        }

        //specifying the name and location for storing Items data
        public static string GetItemsFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "items.json");
        }

        //specifying the name and location for storing Inventory Log data
        public static string GetInventoryLogFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "inventoryLog.json");
        }

        //specifying the name and location for storing Admin Login details
        public static string GetAdminLoginFilePath()
        {
            return Path.Combine(GetJSONfilePath(), "adminLogin.json");
        }
    }
}
