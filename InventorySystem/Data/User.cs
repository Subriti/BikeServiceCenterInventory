namespace InventorySystem.Data
{
    //Declaration of model User and its attributes
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();          //providing default values
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;  //providing default values
        public Guid CreatedBy { get; set; }
    }
}
