using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Data
{
    public class ViewModel
    {
        public ObservableCollection<Model> Data { get; set; }

        public ViewModel()
        {
            Data = new ObservableCollection<Model>();

            //inventory log bata get item name with +quantity sabai each item ko
            var itemLogs = InventoryLogService.GetAll();
            foreach (var item in itemLogs)
            {
                Data.Add(
               new Model(item.ItemName, item.Quantity));
            }
        }
    }
}
