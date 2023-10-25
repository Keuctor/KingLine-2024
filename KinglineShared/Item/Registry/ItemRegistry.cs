
using System.Collections.Generic;

public enum IType : int{ 
    RESOURCE,
    TOOL,
    HELMET,
    ARMOR,
    WEAPON,
}

public static class ItemFactory
{
    public static ItemStack CreateItem(MaterialType material)
    {
        return new ItemStack()
        {
            Count = 1,
            Id = (int) material
        };
    }
    public static ItemStack[] CreateItems(params MaterialType[] materials)
    {
        ItemStack[] items = new ItemStack[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            items[i] = CreateItem(materials[i]);
        }
        return items;
    }
}

public static class ItemRegistry
{
    private static Dictionary<int, IItemMaterial> Materials = new Dictionary<int, IItemMaterial>();
    static ItemRegistry()
    {
        Materials.Add((int)MaterialType.STONE, new ResourceItemMaterial("Stone"));
        Materials.Add((int)MaterialType.BONE, new ResourceItemMaterial("Bone"));
        Materials.Add((int)MaterialType.TOOL_STONE_PICKAXE, new ToolItemMaterial("StonePickaxe", 1.25f));
        Materials.Add((int)MaterialType.TOOL_IRON_PICKAXE, new ToolItemMaterial("IronPickaxe", 2f));
        Materials.Add((int)MaterialType.TOOL_STEEL_PICKAXE, new ToolItemMaterial("SteelPickaxe", 5.25f));
        Materials.Add((int)MaterialType.PEASANT_HELMET, new ArmorItemMaterial("PeasantCap", 3, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.PEASANT_CLOTHING_ARMOR, new ArmorItemMaterial("PeasantClothing", 6, EquipmentSlot.ARMOR));
        Materials.Add((int)MaterialType.LEATHER_HELMET, new ArmorItemMaterial("LeatherHelm", 7, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.LEATHER_JACKET_ARMOR, new ArmorItemMaterial("LeatherJacket", 9, EquipmentSlot.ARMOR));
        Materials.Add((int)MaterialType.CHAINMAIL_HELMET, new ArmorItemMaterial("SteelHelm [Paint]", 12, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.CHAINMAIL_LIGHT_ARMOR, new ArmorItemMaterial("ChainmailLightArmor [Paint]", 15, EquipmentSlot.ARMOR));
        Materials.Add((int)MaterialType.CATAPHRACT_HELMET, new ArmorItemMaterial("CataphractHelm [Paint]", 27, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.CATAPHRACT_ARMOR, new ArmorItemMaterial("CataphractArmor [Paint]", 34, EquipmentSlot.ARMOR));
        Materials.Add((int)MaterialType.ELITE_KNIGHT_HELMET, new ArmorItemMaterial("EliteKnightHelm", 32, EquipmentSlot.HELMET));
        Materials.Add((int)MaterialType.ELITE_GUARD_ARMOR, new ArmorItemMaterial("KnightArmor", 35, EquipmentSlot.ARMOR));
        Materials.Add((int)MaterialType.BONE_CLUP_WEAPON, new WeaponItemMaterial("BoneClub", 4));
        Materials.Add((int)MaterialType.WOODEN_CLUP_WEAPON, new WeaponItemMaterial("WoodenClub", 8));
        Materials.Add((int)MaterialType.GUARD_SWORD_WEAPON, new WeaponItemMaterial("GuardSword1 [Paint]", 14));
        Materials.Add((int)MaterialType.SMALLAXE_WEAPON, new WeaponItemMaterial("SmallAxe", 12));
        Materials.Add((int)MaterialType.KNIGHT_SWORD_WEAPON, new WeaponItemMaterial("KnightSword [Paint]", 18));
    }

    public static int GetMaterialId(MaterialType material)
    {
        return (int)material;
    }

    public static IItemMaterial GetItem(int id)
    {
        if (Materials.TryGetValue(id, out IItemMaterial material)) {
            material.Id = id;
            return material;
        }
        return default;
    }
}
