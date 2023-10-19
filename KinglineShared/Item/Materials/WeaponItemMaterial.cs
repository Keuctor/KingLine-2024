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
        this.Type = IType.WEAPON;
        this.Value = this.Attack * 10;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public IType Type { get; set; }
    public int Attack { get; set; }
    public int Value { get; set; }
}
