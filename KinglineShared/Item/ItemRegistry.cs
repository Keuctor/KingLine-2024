

using KinglineShared.Item;
using System.Collections.Generic;

public enum MaterialType : int
{
    STONE,
    BONE,
    STONE_PICKAXE,
    PEASANT_CAP,
    BONE_CLUP,
    PEASANT_CLOTHING,
    IRON_PICKAXE,
    STEEL_PICKAXE
}

public static class MaterialTypeExtension
{
    public static int ID(this MaterialType material)
    {
        return (int)material;
    }
}
public class ItemRegistry
{

    public Dictionary<int, IItemMaterial> Materials = new Dictionary<int, IItemMaterial>();
    public ItemRegistry()
    {
        Materials.Add((int)MaterialType.STONE, new ItemMaterial("Stone", "Resource"));
        Materials.Add((int)MaterialType.BONE, new ItemMaterial("Bone", "Resource"));
        Materials.Add((int)MaterialType.STONE_PICKAXE, new ToolItemMaterial("StonePickaxe", 1.25f));
        Materials.Add((int)MaterialType.IRON_PICKAXE, new ToolItemMaterial("IronPickaxe", 2.00f));
        Materials.Add((int)MaterialType.STEEL_PICKAXE, new ToolItemMaterial("SteelPickaxe", 3.5f));
        Materials.Add((int)MaterialType.PEASANT_CAP, new ArmorItemMaterial("PeasantCap", 10, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.BONE_CLUP, new WeaponItemMaterial("BoneClub", 10));
        Materials.Add((int)MaterialType.PEASANT_CLOTHING, new ArmorItemMaterial("PeasantClothing", 10, EquipmentSlot.CHEST));
    }

    public int GetMaterialId(MaterialType material)
    {
        return (int)material;
    }

    public IItemMaterial GetItem(int id)
    {
        if (Materials.TryGetValue(id, out IItemMaterial material)) {
            return material;
        }
        return null;
    }
}
