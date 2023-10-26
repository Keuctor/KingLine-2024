using System;
using System.Collections.Generic;
using System.Text;


public interface ITroop
{
    public string Name { get; set; }
    public ItemStack[] Gear { get; set; }
    public int UpgradePrice { get; set; }
    public int NextTroopId { get; set; }
    public int UpgradeXp { get; set; }
}


public enum TroopType : int
{
    PEASANT = 0,
    SWORDSMAN = 1,
    DEFENDER = 2,
    ELITE_SWORDSMAN = 3
}
public static class TroopRegistry
{
    public static Dictionary<int, ITroop> Troops = new Dictionary<int, ITroop>();
    static TroopRegistry()
    {
        Troops.Add((int)TroopType.PEASANT, new Troop()
        {
            Gear = ItemFactory.CreateItems(MaterialType.PEASANT_HELMET,
            MaterialType.PEASANT_CLOTHING_ARMOR, MaterialType.BONE_CLUP_WEAPON),
            Name = "Peasant",
            NextTroopId = 1,
            UpgradePrice = 500,
            UpgradeXp = XPManager.TeamLevels[0],
        });
        Troops.Add((int)TroopType.SWORDSMAN, new Troop()
        {
            Gear = ItemFactory.CreateItems(MaterialType.LEATHER_HELMET,
            MaterialType.LEATHER_JACKET_ARMOR, MaterialType.WOODEN_CLUP_WEAPON),
            Name = "Swordsman",
            NextTroopId = 2,
            UpgradePrice = 1250,
            UpgradeXp = XPManager.TeamLevels[1],
        });
        Troops.Add((int)TroopType.DEFENDER, new Troop()
        {
            Gear = ItemFactory.CreateItems(MaterialType.ELITE_KNIGHT_HELMET,
            MaterialType.ELITE_GUARD_ARMOR, MaterialType.GUARD_SWORD_WEAPON),
            Name = "Defender",
            NextTroopId = 3,
            UpgradePrice = 2650,
            UpgradeXp = XPManager.TeamLevels[2],
        });
        Troops.Add((int)TroopType.ELITE_SWORDSMAN, new Troop()
        {
            Gear = ItemFactory.CreateItems(MaterialType.CATAPHRACT_HELMET,
            MaterialType.CATAPHRACT_ARMOR, MaterialType.KNIGHT_SWORD_WEAPON),
            Name = "Elite Swordsman",
            NextTroopId = -1,
            UpgradePrice = 7500,
            UpgradeXp = XPManager.TeamLevels[3],
        });
    }
    public static ITroop GetTroop(int id)
    {
        return Troops[id];
    }
}
class Troop : ITroop
{
    public string Name { get; set; }
    public ItemStack[] Gear { get; set; }
    public int UpgradePrice { get; set; }
    public int UpgradeXp { get; set; }
    public int NextTroopId { get; set; }
    public Troop()
    {

    }

    public Troop(string name, int upgradePrice, int upgradeXp, int nextTroopId, params ItemStack[] gear)
    {
        this.Name = name;
        this.Gear = gear;
        this.UpgradeXp = upgradeXp;
        this.UpgradePrice = upgradePrice;
        this.NextTroopId = nextTroopId;
    }
}
