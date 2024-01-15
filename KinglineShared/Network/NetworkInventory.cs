

using System;

public class NetworkInventory
{
    public ulong Id { get; set; }
    public string Name { get; set; }

    private ItemStack[] items;
    private ItemStack[] gear;
    public ItemStack[] Items => items;
    public ItemStack[] Gear
    {
        get
        {
            if (gear == null || gear.Length==0)
            {
                gear = new ItemStack[3];
                gear[0] = new ItemStack();
                gear[1] = new ItemStack();
                gear[2] = new ItemStack();
            }
            return gear;
        }
    }

    public const int INVENTORY_SIZE = 20;

    static Random random = new Random();
    public NetworkInventory(string name)
    {
        byte[] buffer = new byte[8];
        random.NextBytes(buffer);
        Id = BitConverter.ToUInt64(buffer, 0);
        this.items = new ItemStack[INVENTORY_SIZE];
        for (int i = 0; i < this.items.Length; i++)
        {
            this.items[i] = new ItemStack()
            {
                Id = -1,
                Count = 0
            };
        }
        Name = name;
    }
    
    public NetworkInventory(string name, ItemStack[] items)
    {
        this.Name = name;
        this.items = items;
    }
    public NetworkInventory(string name, ItemStack[] items, ItemStack[] gear)
    {
        this.Name = name;
        this.items = items;
        this.gear = gear;
    }

    public void SetGear(ItemStack[] gear) {
        this.gear = gear;
    }

    public ItemStack GetHelmet()
    {
        return this.Gear[0];
    }

    public ItemStack GetArmor()
    {
        return this.Gear[1];
    }

    public ItemStack GetHand()
    {
        return this.Gear[2];
    }

    public ItemStack[] GetGear()
    {
        return new ItemStack[3] {
            GetHelmet(),GetArmor(),GetHand()
        };
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
        if (item.Id != -1)
        {
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

        if (itemInfo == null)
            return false;

        if (itemInfo.Stackable)
        {
            var found = FindFirstItemMatchIndex(itemInfo.Id);
            if (found != -1)
            {
                items[found].Count += count;
                return true;
            }
            else
            {
                var empty = NextEmptyInventorySpaceIndex();
                if (empty != -1)
                {
                    items[empty] = new ItemStack()
                    {
                        Count = count,
                        Id = id
                    };
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {

            if (count > GetTotalEmptyInventorySpaces())
                return false;

            while (count > 0)
            {
                var empty = NextEmptyInventorySpaceIndex();
                if (empty != -1)
                {
                    items[empty] = new ItemStack()
                    {
                        Count = 1,
                        Id = itemInfo.Id
                    };
                    count--;
                }
                else
                {
                    break;
                }
            }
            return count == 0;
        }
    }

    public int FindFirstItemMatchIndex(int itemId)
    {
        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item.Id == itemId)
            {
                return i;
            }
        }
        return -1;
    }

    public byte GetTotalEmptyInventorySpaces()
    {
        byte count = 0;

        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].Id == -1)
            {
                count++;
            }
        }
        return count;
    }

    public int NextEmptyInventorySpaceIndex()
    {
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].Id == -1)
            {
                return i;
            }
        }
        return -1;
    }


    public int GetItemCount(int id)
    {
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

    public bool MoveItem(ushort fromIndex, ushort toIndex)
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
