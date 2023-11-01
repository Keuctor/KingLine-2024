using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGearInventoryView : MonoBehaviour
{
    public FilteredItemStackView[] Items;

    [Header("Dependency"), SerializeField]
    private MaterialSpriteDatabase m_spriteDatabase;

    [SerializeField]
    private InventoryNetworkController m_inventoryController;

    [Header("Prefab"), SerializeField]
    private ItemStackContentView m_itemStackContentView;

    private void OnEnable()
    {
        DisplayGear(NetworkManager.LocalPlayerPeerId);
    }


    public void DisplayGear(int id)
    {
        var gear = InventoryNetworkController.GetPlayerGear(id);
        for (int i = 0; i < gear.Length; i++)
        {
            if (gear[i].Id != -1)
            {
                var item = Instantiate(m_itemStackContentView, Items[i].Content);
                item.ItemId = gear[i].Id;
                item.SetContext(m_spriteDatabase.LoadSprite(gear[i].Id), 0, false);
            }
        }
    }
}