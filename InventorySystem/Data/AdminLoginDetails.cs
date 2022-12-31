using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Data
{
    public class AdminLoginDetails
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public DateTime LoggedIn { get; set; } = DateTime.Now;
        public string LoggedOut { get; set; } = "Unavailable";
    }
}
