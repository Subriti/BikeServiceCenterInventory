using System.Text.Json;

namespace InventorySystem.Data;

public static class ItemsService
{
    //writing the Items details to the JSON file
    private static void SaveAll(List<Items> items)
    {
        string appDataDirectoryPath = Utils.GetJSONfilePath();
        string itemsFilePath = Utils.GetItemsFilePath();

        if (!Directory.Exists(appDataDirectoryPath))
        {
            Directory.CreateDirectory(appDataDirectoryPath);
        }

        //converting the List of Items to JSON and writing to the file
        var json = JsonSerializer.Serialize(items);
        File.WriteAllText(itemsFilePath, json);
    }


    //getting all the Items details from the JSON file in a List of type Items
    public static List<Items> GetAll()
    {
        string itemsFilePath = Utils.GetItemsFilePath();
        if (!File.Exists(itemsFilePath))
        {
            return new List<Items>();
        }

        //reading JSON data from the file and converting it to the List of type Items
        var json = File.ReadAllText(itemsFilePath);
        return JsonSerializer.Deserialize<List<Items>>(json);
    }


    //adding new item's details to the JSON file
    public static List<Items> Create(Guid userId, string itemName, int quantity)
    {
        //getting all items
        List<Items> items = GetAll();

        if (itemName != null)
        {

            //checking if item already exists
            var itemNameExists = items.Any(t => t.ItemName.ToLower().Contains(itemName.ToLower()));

            if (itemNameExists)
            {
                throw new Exception("Item already exists in the inventory.");
            }
        }
        else
        {
            throw new Exception("Please fill in the item name.");
        }

        if (quantity < 1)
        {
            throw new Exception("Quantity of items must be minimum of 1 to be added");
        }

        //adding new item to the existing data list
        items.Add(new Items
        {
            ItemName = itemName,
            Quantity = quantity,
            AddedBy = userId
        });

        //saving new details to the JSON file
        SaveAll(items);
        return items;
    }

    //returns one Items Object by checking over the itemName
    public static Items GetItemByName(string itemName)
    {
        return GetAll().FirstOrDefault(t => t.ItemName.ToLower().Contains(itemName.ToLower()));
    }



    //finding the item by id and then deleting the item
    public static List<Items> Delete(Guid id)
    {
        List<Items> items = GetAll();
        Items Item = items.FirstOrDefault(x => x.Id == id);

        if (Item == null)
        {
            throw new Exception("Item not found.");
        }

        //removing the item object from list of items
        items.Remove(Item);

        //saving the updated details to the JSON file
        SaveAll(items);
        return items;
    }


    //finding the item by id and then updating its values
    public static List<Items> Update(Guid id, string itemName, int quantity)
    {
        List<Items> items = GetAll();

        //checking if item exists in the file
        Items itemToUpdate = items.FirstOrDefault(x => x.Id == id);

        if (itemToUpdate == null)
        {
            throw new Exception("Item not found.");
        }

        if (quantity < 1)
        {
            throw new Exception("Quantity of items must be minimum of 1");
        }

        if (itemToUpdate.ItemName != itemName)
        {
            //checking if item name already exists
            var itemNameExists = items.Any(t => t.ItemName.ToLower().Equals(itemName.ToLower()));
            if (itemNameExists)
            {
                throw new Exception("Item with this name already exists in the inventory.");
            }

            //on edit, updating all the existing logs with the new item name
            InventoryLogService.UpdateItemName(itemToUpdate.ItemName, itemName);

            //updating item with new value
            itemToUpdate.ItemName = itemName; 
        }

        //updating item with new value for quantity
        itemToUpdate.Quantity = quantity;

        //saving the updated details to the JSON file
        SaveAll(items);
        return items;
    }

    //finding the item by id and then updating its values after request
    public static List<Items> UpdateQuantityAndLastTakenOut(Guid id, int quantity, DateTime lastTakenOut)
    {
        List<Items> items = GetAll();
        //checking if item exists in the file
        Items itemToUpdate = items.FirstOrDefault(x => x.Id == id);

        if (itemToUpdate == null)
        {
            throw new Exception("Item not found.");
        }

        //decreasing the item quantity on item removal
        itemToUpdate.Quantity -= quantity; 

        //setting the LastTakenAt date to the removal system datetime
        itemToUpdate.LastTakenAt = lastTakenOut;

        //saving the updated details to the JSON file
        SaveAll(items);
        return items;
    }
}
