using System;
using System.Collections.Generic;
using System.Text;

namespace KinglineShared.Item
{
    public class ToolItemMaterial : IItemMaterial
    {
        public ToolItemMaterial(string name, float toolValue)
        {
            this.Name = name;
            this.Stackable = false;
            this.ToolValue = toolValue;
            this.Type = "Tool";
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Stackable { get; set; }
        public string Type { get; set; }
        public float ToolValue { get; set; }
    }

}
