using System.Collections.ObjectModel;

namespace InventorySystem.Data
{
    public class Model
    {
        public string Item { get; set; }

        public int NumberTakenOut { get; set; }

        public Model(string xValue, int yValue)
        {
            Item = xValue;
            NumberTakenOut = yValue;
        }
    }
}
