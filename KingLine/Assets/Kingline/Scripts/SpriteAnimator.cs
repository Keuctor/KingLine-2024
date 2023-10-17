using System;
using Assets.HeroEditor.Common.CharacterScripts;
using Assets.HeroEditor.Common.CommonScripts;
using HeroEditor.Common;
using HeroEditor.Common.Enums;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private Character m_character;

    private Vector3 m_scale;

    private void Start()
    {
        m_scale = m_character.transform.localScale;
        m_character.ResetEquipment();
        OnGearChange();
        InventoryNetworkController.Instance.OnGearChange += OnGearChange;
    }

    private void OnDestroy()
    {
        InventoryNetworkController.Instance.OnGearChange -= OnGearChange;
    }

    private void OnGearChange()
    {
        var items = InventoryNetworkController.Instance.Items;
        if (items[25].Id != -1)
        {
            var helmets = m_character.SpriteCollection.Helmet;
            var itemInfo = InventoryNetworkController.Instance.m_itemInfo.GetItem(items[25].Id);
            for (int i = 0; i < helmets.Count; i++)
            {
                if (helmets[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(helmets[i], EquipmentPart.Helmet);
                    break;
                }
            }
        }
        else
        {
            m_character.UnEquip(EquipmentPart.Helmet);
        }
        
        if (items[26].Id != -1)
        {
            var armor = m_character.SpriteCollection.Armor;
            var itemInfo = InventoryNetworkController.Instance.m_itemInfo.GetItem(items[26].Id);
            for (int i = 0; i < armor.Count; i++)
            {
                if (armor[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(armor[i], EquipmentPart.Armor);
                    break;
                }
            }
        }
        else
        {
            m_character.UnEquip(EquipmentPart.Armor);
        }
      
        if (items[27].Id != -1)
        {
            var weapons = m_character.SpriteCollection.MeleeWeapon1H;
            var itemInfo = InventoryNetworkController.Instance.m_itemInfo.GetItem(items[27].Id);
            for (int i = 0; i < weapons.Count; i++)
            {
                if (weapons[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(weapons[i], EquipmentPart.MeleeWeapon1H);
                    break;
                }
            }
        }
        else
        {
            m_character.UnEquip(EquipmentPart.MeleeWeapon1H);
        }
    }


    public void SetPlay(bool play)
    {
        m_character.SetState(play ? CharacterState.Run : CharacterState.Idle);
    }


    public void SetDirection(MoveDirection moveDirection)
    {
        if (moveDirection == MoveDirection.Right)
        {
            m_character.transform.localScale = new Vector3(m_scale.x, m_scale.y, m_scale.z);
        }
        else if (moveDirection == MoveDirection.Left)
        {
            m_character.transform.localScale = new Vector3(-m_scale.x, m_scale.y, m_scale.z);
        }
    }
}