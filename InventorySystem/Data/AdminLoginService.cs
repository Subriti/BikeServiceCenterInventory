using System.Text.Json;
namespace InventorySystem.Data
{
    public class AdminLoginService
    {
        private static void SaveAll(List<AdminLoginDetails> loginDetails)
        {
            string appDataDirectoryPath = Utils.GetJSONfilePath();
            string loginDetailsFilePath = Utils.GetAdminLoginFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            var json = JsonSerializer.Serialize(loginDetails);
            File.WriteAllText(loginDetailsFilePath, json);
        }

        public static List<AdminLoginDetails> GetAll()
        {
            string loginDetailsFilePath = Utils.GetAdminLoginFilePath();
            if (!File.Exists(loginDetailsFilePath))
            {
                return new List<AdminLoginDetails>();
            }

            var json = File.ReadAllText(loginDetailsFilePath);
            return JsonSerializer.Deserialize<List<AdminLoginDetails>>(json);
        }

        public static List<AdminLoginDetails> Create(Guid userId, string userName)
        {
            List<AdminLoginDetails> loginDetails = GetAll();
            loginDetails.Add(new AdminLoginDetails
            {
                Id = userId,
                Username = userName
            });
            SaveAll(loginDetails);
            return loginDetails;
        }

        public static AdminLoginDetails GetByName(string userName)
        {
            List<AdminLoginDetails> loginDetails = GetAll();
            return loginDetails.LastOrDefault(x => x.Username == userName);
        }

        public static List<AdminLoginDetails> Delete(Guid id)
        {
            List<AdminLoginDetails> loginDetails = GetAll();
            AdminLoginDetails LoginDetails = loginDetails.FirstOrDefault(x => x.Id == id);

            if (LoginDetails == null)
            {
                throw new Exception("Admin Details not found.");
            }

            loginDetails.Remove(LoginDetails);
            SaveAll(loginDetails);
            return loginDetails;
        }


        public static List<AdminLoginDetails> LogOut(Guid id)
        {
            List<AdminLoginDetails> loginDetails = GetAll();
            AdminLoginDetails detailsToUpdate = loginDetails.LastOrDefault(x => x.Id == id);

            if (detailsToUpdate == null)
            {
                throw new Exception("Admin Details not found.");
            }

            detailsToUpdate.LoggedOut = DateTime.Now.ToString();
            SaveAll(loginDetails);
            return loginDetails;
        }
    }
}
