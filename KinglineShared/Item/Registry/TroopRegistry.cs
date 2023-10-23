using System;
using System.Collections.Generic;
using System.Text;


public interface ITroop
{
    public string Name { get; set; }
}

public static class TroopRegistry
{
    public static Dictionary<int, ITroop> Troops = new Dictionary<int, ITroop>();
    static TroopRegistry()
    {
        Troops.Add(0, new Troop("Peasant"));
        Troops.Add(1, new Troop("Swordsman"));
        Troops.Add(2, new Troop("Defender"));
        Troops.Add(3, new Troop("Engineer"));
        Troops.Add(4, new Troop("Archer"));
    }
}
class Troop : ITroop
{
    public string Name { get; set; }

    public Troop(string name)
    {
        this.Name = name;
    }
}
