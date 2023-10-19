using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private Character m_character;

    [SerializeField]
    private Vector3 m_scale = new(0.25f,0.25f,0.25f);

    private InventoryNetworkController m_inventoryNetworkController;


    private void Start()
    {
        m_inventoryNetworkController = NetworkManager.Instance.GetController<InventoryNetworkController>();
        m_inventoryNetworkController.OnGearChange.AddListener(DisplayGear);
        m_character.ResetEquipment();
        DisplayGear();
    }

    private async void DisplayGear()
    {
        var inventory = await InventoryNetworkController.GetInventoryAsync();
        var helmet = inventory.GetHelmet();
        var armor = inventory.GetArmor();
        var hand = inventory.GetHand();
        if (helmet.Id != -1)
        {
            var helmets = m_character.SpriteCollection.Helmet;
            var itemInfo = ItemRegistry.GetItem(helmet.Id);
            for (var i = 0; i < helmets.Count; i++)
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

        if (armor.Id != -1)
        {
            var armors = m_character.SpriteCollection.Armor;
            var itemInfo = ItemRegistry.GetItem(armor.Id);
            for (var i = 0; i < armors.Count; i++)
            {
                if (armors[i].Name.Equals(itemInfo.Name))
                {
                    m_character.Equip(armors[i], EquipmentPart.Armor);
                    break;
                }
            }
        }
        else
        {
            m_character.UnEquip(EquipmentPart.Armor);
        }

        if (hand.Id != -1)
        {
            var weapons = m_character.SpriteCollection.MeleeWeapon1H;
            var itemInfo = ItemRegistry.GetItem(hand.Id);
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