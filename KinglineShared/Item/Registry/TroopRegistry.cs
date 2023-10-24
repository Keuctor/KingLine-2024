using System;
using System.Collections.Generic;
using System.Text;


public interface ITroop
{
    public string Name { get; set; }
    public ItemStack[] Gear { get; set; }
    public float UpgradePrice { get; set; }
    public int NextTroopId { get; set; }
}

public static class TroopRegistry
{
    public static Dictionary<int, ITroop> Troops = new Dictionary<int, ITroop>();
    static TroopRegistry()
    {
        Troops.Add(0, new Troop("Peasant", 500, 1, ItemFactory.CreateItems(MaterialType.PEASANT_HELMET,
            MaterialType.PEASANT_CLOTHING_ARMOR, MaterialType.BONE_CLUP_WEAPON)));

        Troops.Add(1, new Troop("Swordsman", 1250, 2,
            ItemFactory.CreateItems(MaterialType.LEATHER_HELMET,
            MaterialType.LEATHER_JACKET_ARMOR, MaterialType.WOODEN_CLUP_WEAPON)));
        Troops.Add(2, new Troop("Defender", 2650, 3, ItemFactory.CreateItems(MaterialType.ELITE_KNIGHT_HELMET,
            MaterialType.ELITE_GUARD_ARMOR, MaterialType.GUARD_SWORD_WEAPON)));

        Troops.Add(3, new Troop("Elite Swordsman", 7500, 4, ItemFactory.CreateItems(MaterialType.CATAPHRACT_HELMET,
            MaterialType.CATAPHRACT_ARMOR, MaterialType.KNIGHT_SWORD_WEAPON)));
        Troops.Add(4, new Troop("Archer", -1, -1));
    }

}
class Troop : ITroop
{
    public string Name { get; set; }
    public ItemStack[] Gear { get; set; }
    public float UpgradePrice { get; set; }
    public int NextTroopId { get; set; }

    public Troop(string name, float upgradePrice, int nextTroopId, params ItemStack[] gear)
    {
        this.Name = name;
        this.Gear = gear;
        this.UpgradePrice = upgradePrice;
        this.NextTroopId = nextTroopId;
    }
}
