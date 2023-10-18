using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private Character m_character;

    private Vector3 m_scale;

    private InventoryNetworkController m_inventoryNetworkController;

    private ItemRegistry m_itemRegistry;

    private void Start()
    {
        m_inventoryNetworkController = NetworkManager.Instance.GetController<InventoryNetworkController>();
        m_inventoryNetworkController.OnGearChange.AddListener(OnGearChange);
        m_itemRegistry = FindObjectOfType<InventoryController>().ItemRegistry;
        m_scale = m_character.transform.localScale;
        m_character.ResetEquipment();
        OnGearChange();
    }


    private void OnGearChange()
    {
        var items = m_inventoryNetworkController.Items;
        if (items[25].Id != -1)
        {
            var helmets = m_character.SpriteCollection.Helmet;
            var itemInfo = m_itemRegistry.GetItem(items[25].Id);
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
            var itemInfo = m_itemRegistry.GetItem(items[26].Id);
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
            var itemInfo = m_itemRegistry.GetItem(items[27].Id);
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