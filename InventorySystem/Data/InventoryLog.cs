using System.ComponentModel.DataAnnotations;

namespace InventorySystem.Data
{
    public class InventoryLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please provide the item name.")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Please provide the item quantity.")]
        public int Quantity { get; set; }

        public Guid ApprovedBy { get; set; }

        [Required(ErrorMessage = "Please provide the involved staff.")]
        public string TakenBy { get; set; }

        public DateTime TakenOutAt { get; set; } = DateTime.Now;
    }
}
