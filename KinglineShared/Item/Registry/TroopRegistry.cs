using System;
using System.Collections.Generic;
using System.Text;


public interface ITroop
{
    public string Name { get; set; }
    public ItemStack[] Gear { get; set; }
}

public static class TroopRegistry
{
    public static Dictionary<int, ITroop> Troops = new Dictionary<int, ITroop>();
    static TroopRegistry()
    {
        //MaterialType.PEASANT_CAP,MaterialType.PEASANT_CLOTHING,MaterialType.BONE_CLUP
        Troops.Add(0, new Troop("Peasant", ItemFactory.CreateItems(MaterialType.PEASANT_HELMET,
            MaterialType.PEASANT_CLOTHING_ARMOR, MaterialType.BONE_CLUP_WEAPON)));

        Troops.Add(1, new Troop("Swordsman",
            ItemFactory.CreateItems(MaterialType.LEATHER_HELMET,
            MaterialType.LEATHER_JACKET_ARMOR,MaterialType.WOODEN_CLUP_WEAPON)));
        Troops.Add(2, new Troop("Defender",ItemFactory.CreateItems(MaterialType.ELITE_KNIGHT_HELMET,
            MaterialType.ELITE_GUARD_ARMOR,MaterialType.GUARD_SWORD_WEAPON)));

        Troops.Add(3, new Troop("Elite Swordsman",ItemFactory.CreateItems(MaterialType.CATAPHRACT_HELMET,
            MaterialType.CATAPHRACT_ARMOR,MaterialType.KNIGHT_SWORD_WEAPON)));
        Troops.Add(4, new Troop("Archer"));
    }

}
class Troop : ITroop
{
    public string Name { get; set; }
    public ItemStack[] Gear { get; set; }
    public Troop(string name, params ItemStack[] gear)
    {
        this.Name = name;
        Gear = gear;
    }
}
