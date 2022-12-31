using System.Text.Json;

namespace InventorySystem.Data;

public static class InventoryLogService
{
    private static void SaveAll(List<InventoryLog> itemLogs)
    {
        string appDataDirectoryPath = Utils.GetJSONfilePath();
        string itemsFilePath = Utils.GetInventoryLogFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(itemLogs);
        File.WriteAllText(itemsFilePath, json);
    }

    public static List<InventoryLog> GetAll()
    {
        string itemsFilePath = Utils.GetInventoryLogFilePath();
        if (!File.Exists(itemsFilePath))
        {
            return new List<InventoryLog>();
        }

        var json = File.ReadAllText(itemsFilePath);
        return JsonSerializer.Deserialize<List<InventoryLog>>(json);
    }

    public static List<InventoryLog> GetAllByMonth(string month)
    {
        List<InventoryLog> log = GetAll();
        List<InventoryLog> sortedByMonth = new List<InventoryLog>();
        foreach (var logItem in log)
        {
            //logItem.TakenOutAt.ToString("MMMM") gives January, February
            if (logItem.TakenOutAt.ToString("MMMM").Equals(month))
            {
                //adding sortedData to a new List
                sortedByMonth.Add(logItem);
            }

        }
        return sortedByMonth;
    }

    public static Dictionary<string, int> GetTotalQuantityTakenOut()
    {
        List<InventoryLog> log = GetAll();

        Dictionary<string, int> itemAndQuantity = new Dictionary<string, int>();

        foreach (var logItem in log)
        {
            if (!itemAndQuantity.ContainsKey(logItem.ItemName))
            {
                itemAndQuantity.Add(logItem.ItemName, logItem.Quantity);
            }

            else if (itemAndQuantity.ContainsKey(logItem.ItemName))
            {
                //getting existing quantity
                var quantity = itemAndQuantity[logItem.ItemName];
                //adding quantities 
                itemAndQuantity[logItem.ItemName] = quantity + logItem.Quantity;
            }
        }
        return itemAndQuantity;
    }


    public static InventoryLog GetByName(string itemName)
    {
        List<InventoryLog> log = GetAll();
        return log.LastOrDefault(x => x.ItemName == itemName);
    }

    public static List<InventoryLog> Create(Guid userId, string itemName, int quantity, string takenBy)
    {
        var weekDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        var dayOfWeek = DateTime.Now.DayOfWeek.ToString();

        if (!weekDays.Contains(dayOfWeek))
        {
            throw new Exception("Items cannot be requested on " + dayOfWeek);
        }

        TimeSpan today = DateTime.Now.TimeOfDay;    // 12/29/2022 6:52:36 PM
        int nowHrs = (int)today.TotalHours;        // 18:52:36.9759334
        int startTime = 09;     // 09:00 am
        int endTime = 16;      // 04:00 pm

        if (nowHrs > endTime || nowHrs < startTime)
        {
            throw new Exception("Items can only be requested between 9 am to 4 pm");
        }


        List<Items> items = ItemsService.GetAll();
        bool itemNameExists = items.Any(x => x.ItemName == itemName);

        if (!itemNameExists)
        {
            throw new Exception("The item " + itemName + " does not exist in the inventory");
        }


        if (quantity < 1)
        {
            throw new Exception("Quantity must be minimum of 1 for request");
        }

        var currentItem = ItemsService.GetByName(itemName);
        if (quantity > currentItem.Quantity)
        {
            throw new Exception("Requested quantity is currently unavailable.");
        }

        List<User> user = UsersService.GetAll();
        bool userNameExists = user.Any(x => x.Username == takenBy);

        if (!userNameExists)
        {
            throw new Exception("The user " + takenBy + " does not exist in the system");
        }

        List<InventoryLog> itemLog = GetAll();
        itemLog.Add(new InventoryLog
        {
            ItemName = itemName,
            Quantity = quantity,
            ApprovedBy = userId,
            TakenBy = takenBy
        });
        SaveAll(itemLog);

        //after saving the item log, updating the quantity and LastTakenOut in Items
        var currentLog = InventoryLogService.GetByName(itemName);

        ItemsService.Update(currentItem.Id, quantity, currentLog.TakenOutAt);

        return itemLog;
    }

    public static List<InventoryLog> Delete(Guid id)
    {
        List<InventoryLog> items = GetAll();
        InventoryLog Item = items.FirstOrDefault(x => x.Id == id);

        if (Item == null)
        {
            throw new Exception("Item log not found.");
        }

        items.Remove(Item);
        SaveAll(items);
        return items;
    }


    public static List<InventoryLog> Update(Guid id, string taskName, int quantity, string takenBy)
    {
        List<InventoryLog> items = GetAll();
        InventoryLog itemToUpdate = items.FirstOrDefault(x => x.Id == id);

        if (itemToUpdate == null)
        {
            throw new Exception("Item Log not found.");
        }

        itemToUpdate.ItemName = taskName;
        itemToUpdate.Quantity = quantity;
        itemToUpdate.TakenBy = takenBy;
        SaveAll(items);
        return items;
    }
}
