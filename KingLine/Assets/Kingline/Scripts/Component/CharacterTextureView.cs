using System;
using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTextureView : MonoBehaviour
{
    
    [SerializeField]
    private Character m_characterTemplate;
    
    [SerializeField]
    private RawImage m_rawImage;
    
    private Character m_character;
    
    private RenderTexture m_texture;


    public int Width = 256;

    public int Height = 256;

    public int Depth = 1;
    
    public int Armor;
    
    public int Strength;

    private Camera m_camera;

    public float OrthographicSize = 1.9f;

    private bool m_initialized;

    private void Initialize()
    {
        if (m_initialized)
            return;
        m_texture = new RenderTexture(Width, Height, Depth);
        var cameraObject = new GameObject("CharacterTextureViewCamera");
        m_camera = cameraObject.AddComponent<Camera>();
        m_camera.targetTexture = m_texture;
        m_character = Instantiate(m_characterTemplate);
        m_rawImage.texture = m_texture;

        m_camera.transform.position = new Vector3(9999, 9999, -2);
        m_character.transform.position = new Vector3(9999, 9998, 0);

        m_camera.orthographic = true;
        m_camera.orthographicSize = OrthographicSize;
        m_initialized = true;
    }

    private void OnDisable()
    {
        Clear();
    }

    private void Clear()
    {
        if (m_camera)
        {
            Destroy(m_camera.gameObject);
            m_camera = null;
        }
        
        if (m_character)
        {
            Destroy(m_character.gameObject);
            m_character = null;
        }

        if (m_texture)
        {
            Destroy(m_texture);
            m_texture = null;
        }

        m_initialized = false;
    }

    public void Show(ItemStack[] items)
    {
        Initialize();
        
        m_character.ResetEquipment();
        var helmet = items[0].Id;
        var armor = items[1].Id;
        var hand = items[2].Id;

        if (helmet != -1)
        {
            var helmets = m_character.SpriteCollection.Helmet;
            var itemInfo = ItemRegistry.GetItem(helmet);
            for (var i = 0; i < helmets.Count; i++)
                if (helmets[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(helmets[i], EquipmentPart.Helmet);
                    break;
                }
            
            var armorMaterial = (ArmorItemMaterial)itemInfo;
            Armor += (byte)armorMaterial.Armor;
        }
        else
        {
            m_character.UnEquip(EquipmentPart.Helmet);
        }

        if (armor != -1)
        {
            var armors = m_character.SpriteCollection.Armor;
            var itemInfo = ItemRegistry.GetItem(armor);
            for (var i = 0; i < armors.Count; i++)
                if (armors[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(armors[i], EquipmentPart.Armor);
                    break;
                }
            
            var armorMaterial = (ArmorItemMaterial)itemInfo;
            Armor += (byte)armorMaterial.Armor;
        }
        else
        {
            m_character.UnEquip(EquipmentPart.Armor);
        }

        if (hand != -1)
        {
            var weapons = m_character.SpriteCollection.MeleeWeapon1H;
            var itemInfo = ItemRegistry.GetItem(hand);
            for (var i = 0; i < weapons.Count; i++)
                if (weapons[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(weapons[i], EquipmentPart.MeleeWeapon1H);
                    break;
                }
            
            var armorMaterial = (WeaponItemMaterial)itemInfo;
            Strength += (byte)armorMaterial.Attack;
        }
        else
        {
            m_character.UnEquip(EquipmentPart.MeleeWeapon1H);
        }
    }
}