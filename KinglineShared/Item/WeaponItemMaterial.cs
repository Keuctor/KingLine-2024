using System;
using System.Collections.Generic;
using System.Text;


public class WeaponItemMaterial : IWeaponItemMaterial
{
    public WeaponItemMaterial(string name, int attackValue)
    {
        this.Name = name;
        this.Stackable = false;
        this.Attack = attackValue;
        this.Type = "Weapon";
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public string Type { get; set; }
    public int Attack { get; set; }
}
