using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Data
{
    public class Items
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please provide the item name.")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Please provide the item quantity.")]
        public int Quantity { get; set; }

        public Guid AddedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime LastTakenAt { get; set; }
    }
}
