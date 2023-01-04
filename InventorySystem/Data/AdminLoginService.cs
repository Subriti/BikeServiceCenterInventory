using System.Text.Json;
namespace InventorySystem.Data
{
    public class AdminLoginService
    {
        //writing the login details to the JSON file
        private static void SaveAll(List<AdminLoginDetails> loginDetails)
        {
            string appDataDirectoryPath = Utils.GetJSONfilePath();
            string loginDetailsFilePath = Utils.GetAdminLoginFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            //converting the List of AdminLoginDetails to JSON and writing to the file
            var json = JsonSerializer.Serialize(loginDetails);
            File.WriteAllText(loginDetailsFilePath, json);
        }


        //getting all the loginDetails from the JSON file in a List of type AdminLoginDetails
        public static List<AdminLoginDetails> GetAll()
        {
            string loginDetailsFilePath = Utils.GetAdminLoginFilePath();
            if (!File.Exists(loginDetailsFilePath))
            {
                return new List<AdminLoginDetails>();
            }

            //reading JSON data from the file and converting it to the List of type AdminLoginDetails
            var json = File.ReadAllText(loginDetailsFilePath);
            return JsonSerializer.Deserialize<List<AdminLoginDetails>>(json);
        }


        //adding admin Login Details to the JSON file
        public static List<AdminLoginDetails> Create(Guid userId, string userName)
        {
            List<AdminLoginDetails> loginDetails = GetAll();

            //adding new detail to the existing data list
            loginDetails.Add(new AdminLoginDetails
            {
                Id = userId,
                Username = userName
            });

            //saving new details to the JSON file
            SaveAll(loginDetails);
            return loginDetails;
        }


        //updating the loggedOut details of the currentUser on the press of logout button
        public static List<AdminLoginDetails> LogOut(Guid id)
        {
            List<AdminLoginDetails> loginDetails = GetAll();

            //getting the last matching element/user detail of the file because there are many records of the same user and
            //we are only concerned with the recent details of the user
            AdminLoginDetails detailsToUpdate = loginDetails.LastOrDefault(x => x.Id == id);

            if (detailsToUpdate == null)
            {
                throw new Exception("Admin Details not found.");
            }

            //setting loggedOut to the captured system date time at the time of logout
            detailsToUpdate.LoggedOut = DateTime.Now.ToString();

            //saving updated details into the JSON file
            SaveAll(loginDetails);
            return loginDetails;
        }
    }
}
