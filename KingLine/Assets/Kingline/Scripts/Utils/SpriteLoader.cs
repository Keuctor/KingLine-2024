using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class SpriteItem
{
    public MaterialType Type;
    public Sprite Sprite;
}

[CreateAssetMenu]
public class SpriteLoader : ScriptableObject
{
    [SerializeField]
    private SpriteItem[] m_sprites;

    public Sprite LoadSprite(int id)
    {
        return LoadSprite((MaterialType)id);
    }

    public  Sprite LoadSprite(MaterialType type)
    {
        return m_sprites.FirstOrDefault(t => t.Type == type)?.Sprite;
    }

    [Button]
    public void CreateObjects()
    {
        string n = "";
        foreach (var m in m_sprites)
        {
            var item = ItemRegistry.GetItem(ItemRegistry.GetMaterialId(m.Type));
            
            var typeString = m.Type.ToString().ToLower();
            if (typeString.Contains("armor") || typeString.Contains("helmet"))
            {
                var value = -1f;
                var equipmentSlot = EquipmentSlot.HELMET;
                
                if (item is ArmorItemMaterial armor)
                {
                    value = armor.Armor;
                    equipmentSlot = armor.EquipmentSlot;
                }
                n +=
                    $"Materials.Add((int)MaterialType.{m.Type}, new ArmorItemMaterial(\"{m.Sprite.name}\", {value}, EquipmentSlot.{equipmentSlot}));\n";
               
            }
            else if (typeString.Contains("weapon"))
            {
                var value = -1f;
                if (item is WeaponItemMaterial weapon)
                {
                    value = weapon.Attack;
                }
                n +=
                    $" Materials.Add((int)MaterialType.{m.Type}, new WeaponItemMaterial(\"{m.Sprite.name}\", {value}));\n";
            }
            else if(typeString.Contains("tool"))
            {
                var value = -1f;
                
                if (item is ToolItemMaterial tool)
                {
                    value = tool.ToolValue;
                }
                n += $"Materials.Add((int)MaterialType.{m.Type}, new ToolItemMaterial(\"{m.Sprite.name}\", {value}f));\n";
            }
            else {
                n += $"$ Materials.Add((int)MaterialType.{m.Type}, new ResourceItemMaterial(\"{m.Sprite.name}\"));\n";
            }
        }

        n = n.Replace("$","");
        Debug.Log(n);
    }
}