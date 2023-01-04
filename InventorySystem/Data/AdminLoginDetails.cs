namespace InventorySystem.Data
{

    //Declaration of model AdminLoginDetails and its attributes
    public class AdminLoginDetails
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime LoggedIn { get; set; } = DateTime.Now;  //providing default values
        public string LoggedOut { get; set; } = "Unavailable"; //providing default values
    }
}
