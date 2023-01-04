using System.Text.Json;

namespace InventorySystem.Data;

public static class InventoryLogService
{
    //writing data to JSON file
    private static void SaveAll(List<InventoryLog> itemLogs)
    {
        string appDataDirectoryPath = Utils.GetJSONfilePath();
        string itemsFilePath = Utils.GetInventoryLogFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        //converting the List of InventoryLog to JSON and writing to the file
        var json = JsonSerializer.Serialize(itemLogs);
        File.WriteAllText(itemsFilePath, json);
    }

    //getting data from JSON file and storing into a List of type InventoryLog
    public static List<InventoryLog> GetAll()
    {
        string itemsFilePath = Utils.GetInventoryLogFilePath();
        if (!File.Exists(itemsFilePath))
        {
            return new List<InventoryLog>();
        }

        //reading JSON data from the file and converting it to the List of type InventoryLog
        var json = File.ReadAllText(itemsFilePath);
        return JsonSerializer.Deserialize<List<InventoryLog>>(json);
    }


    //For Sorting the InventoryLog according to the Month
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

    //For plotting the graph; calculating the items and the total quantity taken out
    public static Dictionary<string, int> GetTotalQuantityTakenOut()
    {
        List<InventoryLog> log = GetAll();

        //initialising a dictionary
        Dictionary<string, int> itemAndQuantity = new Dictionary<string, int>();

        foreach (var logItem in log)
        {
            //if itemName does not exist in the dictionary, add it to the dictionary
            if (!itemAndQuantity.ContainsKey(logItem.ItemName))
            {
                itemAndQuantity.Add(logItem.ItemName, logItem.Quantity);
            }

            //if itemName exists in the dictionary, add its quantity and update the value of the key
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

    //storing all the months in a list for populating the drop down with options
    public static List<String> GetMonths()
    {
        List<String> months= new List<String>() {"January","February","March","April","May","June","July","August","September","October","November","December"};
        return months;
    }


    //returning the InventoryLog object having itemName; Using Last because
    //we need the last item added to the List for updating its values in items JSON file
    public static InventoryLog GetByName(string itemName)
    {
        return GetAll().LastOrDefault(t => t.ItemName.ToLower().Contains(itemName.ToLower()));
    }


    //creating item requests and adding to the JSON file
    public static List<InventoryLog> Create(Guid userId, string itemName, int quantity, string takenBy)
    {
        //validation for making requests only between Monday to Friday
        var weekDays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        var dayOfWeek = DateTime.Now.DayOfWeek.ToString();

        //if weekDays does not contain the current week day, throw exception
        if (!weekDays.Contains(dayOfWeek))
        {
            throw new Exception("Items cannot be requested on " + dayOfWeek);
        }

        //validation for making requests only between 9:00 am to 4:00 pm
        TimeSpan today = DateTime.Now.TimeOfDay;    // 18:52:36.9759334
        int nowHrs = (int)today.TotalHours;        // 18
        int startTime = 09;                      // 09:00 am
        int endTime = 16;                       // 04:00 pm

        //if nowHrs is greater or equal to 16 or nowHrs is lesser than 9, throw exception
        if (nowHrs >= endTime || nowHrs < startTime)
        {
            throw new Exception("Items can only be requested between 9 am to 4 pm");
        }


        //checking if the item exists in the inventory; .Any returns bool
        var itemNameExists = ItemsService.GetAll().Any(t => t.ItemName.ToLower().Contains(itemName.ToLower()));

        if (!itemNameExists)
        {
            throw new Exception("The item " + itemName + " does not exist in the inventory");
        }

        if (quantity < 1)
        {
            throw new Exception("Quantity must be minimum of 1 for request");
        }

        //checking if the item requested has enough quantity in the inventory
        var currentItem = ItemsService.GetItemByName(itemName);
        if (quantity > currentItem.Quantity)
        {
            throw new Exception("Requested quantity is currently unavailable.");
        }

        //checking if the user exists in the system; .Any returns bool
        bool userNameExists = UsersService.GetAll().Any(x => x.Username == takenBy);

        if (!userNameExists)
        {
            throw new Exception("The user " + takenBy + " does not exist in the system");
        }

        List<InventoryLog> itemLog = GetAll();

        //adding new item log to the existing data list
        itemLog.Add(new InventoryLog
        {
            ItemName = currentItem.ItemName,
            Quantity = quantity,
            ApprovedBy = userId,
            TakenBy = takenBy
        });
        //saving the item log to the JSON file,
        SaveAll(itemLog);

        //accessing the saved current log by name
        var currentLog = GetByName(itemName);

        //updating the quantity and LastTakenOut of the currentLog in Items
        ItemsService.UpdateQuantityAndLastTakenOut(currentItem.Id, quantity, currentLog.TakenOutAt);

        return itemLog;
    }


    //on change of item name in itemService, changing the name in the existing Logs
    public static List<InventoryLog> UpdateItemName(string previousItemName, string newItemName)
    {
        List<InventoryLog> logs = GetAll();

        //looping over the whole list
        foreach(var itemLog in logs)
        {
            //checking if the existing logs contains the item name
            if (itemLog.ItemName.ToLower().Equals(previousItemName.ToLower()))
            {
                //changing existing log's name to the new name
                itemLog.ItemName = newItemName;
                SaveAll(logs);
            }
        }
        return logs;
    }
}
