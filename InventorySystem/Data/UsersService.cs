using System.Text.Json;

namespace InventorySystem.Data
{
    internal class UsersService
    {
        public const string SeedUsername = "admin";
        public const string SeedPassword = "admin";

        
        //writing the user details to the JSON file
        private static void SaveAll(List<User> users)
        {
            string appDataDirectoryPath = Utils.GetJSONfilePath();
            string appUsersFilePath = Utils.GetAppUsersFilePath();

            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }

            //converting the List of User to JSON and writing to the file
            var json = JsonSerializer.Serialize(users);
            File.WriteAllText(appUsersFilePath, json);
        }


        //getting all the User details from the JSON file in a List of type User
        public static List<User> GetAll()
        {
            string appUsersFilePath = Utils.GetAppUsersFilePath();
            if (!File.Exists(appUsersFilePath))
            {
                return new List<User>();
            }

            //reading JSON data from the file and converting it to the List of type User
            var json = File.ReadAllText(appUsersFilePath);
            return JsonSerializer.Deserialize<List<User>>(json);
        }


        //adding new user's details to the JSON file
        public static List<User> Create(Guid userId, string username, string password, Role role)
        {
            //getting all users
            List<User> users = GetAll();

            //checking if username already exists
            bool usernameExists = users.Any(x => x.Username == username);

            if (usernameExists)
            {
                throw new Exception("Username already exists.");
            }

            if (role == Role.Admin)
            {
                //counting the number of admin present in the system
                int adminCount = users.Count(x => x.Role == role);
                if (adminCount == 2)
                {
                    throw new Exception("System cannot have more than two admin");
                }
            }

            //adding new user to the existing data list
            users.Add(
                new User
                {
                    Username = username,
                    PasswordHash = Utils.HashSecret(password),
                    Role = role,
                    CreatedBy = userId
                }
            );

            //saving new details to the JSON file
            SaveAll(users);
            return users;
        }


        //if no user exists in the system, create/seed admin user
        public static void SeedUsers()
        {
            var users = GetAll().FirstOrDefault(x => x.Role == Role.Admin);

            if (users == null)
            {
                Create(Guid.Empty, SeedUsername, SeedPassword, Role.Admin);
            }
        }


        //getting one User detail by id
        public static User GetById(Guid id)
        {
            List<User> users = GetAll();
            return users.FirstOrDefault(x => x.Id == id);
        }

        
        //finding the user by id and then deleting the user
        public static List<User> Delete(Guid id)
        {
            List<User> users = GetAll();
            User user = users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            //removing the user object from list of users
            users.Remove(user);

            //saving the updated details to the JSON file
            SaveAll(users);

            return users;
        }


        // verifying the user at the time of login
        public static User Login(string username, string password)
        {
            var loginErrorMessage = "Invalid username or password.";
            List<User> users = GetAll();
            User user = users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                throw new Exception(loginErrorMessage);
            }

            //matching the user-written password with the one stored in JSON file by verifying the hashes for both passwords
            bool passwordIsValid = Utils.VerifyHash(password, user.PasswordHash);

            if (!passwordIsValid)
            {
                throw new Exception(loginErrorMessage);
            }
            return user;
        }


        //logic to change the user password
        public static User ChangePassword(Guid id, string currentPassword, string newPassword)
        {
            if (currentPassword == newPassword)
            {
                throw new Exception("New password must be different from current password.");
            }

            List<User> users = GetAll();
            User user = users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            //matching the user-written password with the one stored in JSON file by verifying the hashes for both passwords
            bool passwordIsValid = Utils.VerifyHash(currentPassword, user.PasswordHash);

            if (!passwordIsValid)
            {
                throw new Exception("Incorrect current password.");
            }

            //hashing the user-written new password and then saving it to the JSON file
            user.PasswordHash = Utils.HashSecret(newPassword);
            SaveAll(users);

            return user;
        }
    }

}
