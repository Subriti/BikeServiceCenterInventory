using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Data
{
    //Declaration of model Items and its attributes
    public class Items
    {
        public Guid Id { get; set; } = Guid.NewGuid();                //providing default values

        [Required(ErrorMessage = "Please provide the item name.")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Please provide the item quantity.")]
        public int Quantity { get; set; }

        public Guid AddedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;           //providing default values

        public DateTime LastTakenAt { get; set; }
    }
}
