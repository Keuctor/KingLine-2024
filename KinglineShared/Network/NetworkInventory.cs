

using System.Drawing;

public class NetworkInventory
{
    private ItemStack[] items;
    public ItemStack[] Items => items;

    public const int HELMET_SLOT_INDEX = 25;
    public const int ARMOR_SLOT_INDEX = 26;
    public const int HAND_SLOT_INDEX = 27;
    public const int INVENTORY_SIZE = 28;

    public NetworkInventory()
    {
        this.items = new ItemStack[INVENTORY_SIZE];
        for (int i = 0; i < this.items.Length; i++)
        {
            this.items[i] = new ItemStack()
            {
                Id = -1,
                Count = 0
            };
        }
    }

    public NetworkInventory(ItemStack[] items)
    {
        this.items = items;
    }

    public ItemStack GetHelmet()
    {
        return this.items[HELMET_SLOT_INDEX];
    }

    public ItemStack GetArmor()
    {
        return this.items[ARMOR_SLOT_INDEX];
    }

    public ItemStack GetHand()
    {
        return this.items[HAND_SLOT_INDEX];
    }

    
    public void SetItem(int index, int id, short count = 1)
    {
        this.items[index] = new ItemStack()
        {
            Id = id,
            Count = count
        };
    }

    public bool RemoveItem(int index, short count = 1)
    {
        var item = items[index];
        if (item.Id != -1) {
            item.Count -= count;
            if (item.Count <= 0)
            {
                items[index] = new ItemStack()
                {
                    Count = 0,
                    Id = -1
                };
            }
            return true;
        }
        return false;
    }

    public bool AddItem(int id, short count = 1)
    {
        var itemInfo = ItemRegistry.GetItem(id);
        
        for (var i = 0; i < items.Length; i++)
        {
            if (i == HELMET_SLOT_INDEX || i == HAND_SLOT_INDEX || i == ARMOR_SLOT_INDEX)
                continue;

            var item = items[i];
            if (item.Id == id && itemInfo.Stackable)
            {
                item.Count += count;
                return true;
            }
        }
        for (var i = 0; i < items.Length; i++)
        {
            if (i == HELMET_SLOT_INDEX || i == HAND_SLOT_INDEX || i == ARMOR_SLOT_INDEX)
                continue;

            if (items[i].Id == -1)
            {
                items[i].Id = id;
                items[i].Count = count;
                return true;
            }
        }
        return false;
    }
    public int GetItemCount(int id) {
        var total = 0;
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item.Id == id)
            {
                total += item.Count;
            }
        }
        return total;
    }

    public bool MoveItem(short fromIndex, short toIndex)
    {
        var item = items[fromIndex];
        if (item.Id != -1)
        {
            var to = items[toIndex];
            if (to.Id == -1)
            {
                items[toIndex] = item;
                items[fromIndex] = new ItemStack()
                {
                    Count = 0,
                    Id = -1
                };
                return true;
            }
        }
        return false;
    }
}
