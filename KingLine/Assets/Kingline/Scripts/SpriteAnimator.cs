using Assets.HeroEditor.Common.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private Character m_character;

    [SerializeField]
    private Vector3 m_scale = new(0.25f, 0.25f, 0.25f);

    public int PeerId;

    private InventoryNetworkController m_inventoryNetworkController;

    private void Awake()
    {
        m_inventoryNetworkController = NetworkManager.Instance.GetController<InventoryNetworkController>();
        m_inventoryNetworkController.OnGearChange.AddListener(DisplayGear);
        m_character.ResetEquipment();
    }

    private void DisplayGear(int id)
    {
        if (id != PeerId)
            return;

        var inventory = InventoryNetworkController.GetPlayerGear(id);

        var helmet = inventory[0].Id;
        var armor = inventory[1].Id;
        var hand = inventory[2].Id;

        Debug.Log($"PeerId: {PeerId} -> [{helmet}][{armor}][{hand}]");
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
            m_character.transform.localScale = new Vector3(m_scale.x, m_scale.y, m_scale.z);
        else if (moveDirection == MoveDirection.Left)
            m_character.transform.localScale = new Vector3(-m_scale.x, m_scale.y, m_scale.z);
    }
}