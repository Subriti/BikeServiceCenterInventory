using System.Text.Json;

namespace InventorySystem.Data;

public static class ItemsService
{
    private static void SaveAll(List<Items> items)
    {
        string appDataDirectoryPath = Utils.GetJSONfilePath();
        string itemsFilePath = Utils.GetItemsFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        var json = JsonSerializer.Serialize(items);
        File.WriteAllText(itemsFilePath, json);
    }

    public static List<Items> GetAll()
    {
        string itemsFilePath = Utils.GetItemsFilePath();
        if (!File.Exists(itemsFilePath))
        {
            return new List<Items>();
        }

        var json = File.ReadAllText(itemsFilePath);
        return JsonSerializer.Deserialize<List<Items>>(json);
    }

    public static List<Items> Create(Guid userId, string itemName, int quantity)
    {
        if (quantity < 1)
        {
            throw new Exception("Quantity must be minimum of 1 to add item");
        }

        List<Items> items = GetAll();
        items.Add(new Items
        {
            ItemName = itemName,
            Quantity = quantity,
            AddedBy = userId
        });
        SaveAll(items);
        return items;
    }

    //returns one Items Object
    public static Items GetItemByName(string itemName)
    {
        return GetAll().FirstOrDefault(t => t.ItemName.ToLower().Contains(itemName.ToLower()));
    }

    public static List<Items> Delete(Guid id)
    {
        List<Items> items = GetAll();
        Items Item = items.FirstOrDefault(x => x.Id == id);

        if (Item == null)
        {
            throw new Exception("Item not found.");
        }

        items.Remove(Item);
        SaveAll(items);
        return items;
    }


    public static List<Items> Update(Guid id, string taskName, int quantity)
    {
        List<Items> items = GetAll();
        Items itemToUpdate = items.FirstOrDefault(x => x.Id == id);

        if (itemToUpdate == null)
        {
            throw new Exception("Item not found.");
        }

        if (quantity < 1)
        {
            throw new Exception("Quantity must be minimum of 1 to add item");
        }

        itemToUpdate.ItemName = taskName;
        itemToUpdate.Quantity = quantity;
        SaveAll(items);
        return items;
    }

    public static List<Items> Update(Guid id, int quantity, DateTime lastTakenOut)
    {
        List<Items> items = GetAll();
        Items itemToUpdate = items.FirstOrDefault(x => x.Id == id);

        if (itemToUpdate == null)
        {
            throw new Exception("Item not found.");
        }

        itemToUpdate.Quantity -= quantity; 
        itemToUpdate.LastTakenAt = lastTakenOut;
        SaveAll(items);
        return items;
    }
}
