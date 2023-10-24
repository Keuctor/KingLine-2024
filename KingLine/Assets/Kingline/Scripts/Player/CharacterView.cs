using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [SerializeField]
    private GameObject m_characterCamera;

    private Character m_character;

    [SerializeField]
    private TMP_Text m_nameLabel;
    
    [SerializeField]
    private Slider m_xpSlider;
    
    [SerializeField]
    private TMP_Text m_xpSliderText;

    [SerializeField]
    private TMP_Text m_coinLabel;

    [SerializeField]
    private TMP_Text m_armorText;

    [SerializeField]
    private TMP_Text m_weaponText;
    
    private GameObject m_charCamera;

  
    public void Show(string name,ItemStack[] items)
    {
        m_charCamera= Instantiate(m_characterCamera);
        m_charCamera.transform.position = new Vector3(9999,9999,0);
        m_character = m_charCamera.transform.GetChild(0).GetComponent<Character>();
        
        m_nameLabel.text = name;
        m_character.ResetEquipment();
        var helmet = items[0].Id;
        var armor = items[1].Id;
        var hand = items[2].Id;

        var baseArmor = 0;
        var baseStrength = 0;
        
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
            baseArmor += (byte)armorMaterial.Armor;
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
            baseArmor += (byte)armorMaterial.Armor;
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
            baseStrength += (byte)armorMaterial.Attack;
        }
        else
        {
            m_character.UnEquip(EquipmentPart.MeleeWeapon1H);
        }
        
        this.m_armorText.text = baseArmor+"";
        this.m_weaponText.text = baseStrength + "";
    }
    
    private void OnDestroy()
    {
        Destroy(m_charCamera.gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}