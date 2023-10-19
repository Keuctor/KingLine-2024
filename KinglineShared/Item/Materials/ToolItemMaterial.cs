using System;
using System.Collections.Generic;
using System.Text;

public class ToolItemMaterial : IItemMaterial
{
    public ToolItemMaterial(string name, float toolValue)
    {
        Name = name;
        Stackable = false;
        ToolValue = toolValue;
        Type = IType.TOOL;
        this.Value = (int)((ToolValue*5)*1.5f);
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public bool Stackable { get; set; }
    public IType Type { get; set; }
    public float ToolValue { get; set; }
    public int Value { get; set; }
}

