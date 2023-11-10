using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class InventoryView : MonoBehaviour
{
    public bool Modifiable = true;

    public static UnityEvent<int> OnItemClick = new();
    public  UnityEvent<int> OnItemSelect = new();

    [Header("Dependency")]
    [SerializeField]
    private MaterialSpriteDatabase m_spriteDatabase;

    [Header("Prefabs")]
    [SerializeField]
    public ItemStackView m_itemStackTemplate;

    [SerializeField]
    private ItemInfoView m_itemInfoView;

    [SerializeField]
    private ItemStackContentView m_itemStackContentView;

    [Header("Parents")]
    [SerializeField]
    public Transform m_itemStackViewParent;

    public bool ShowGear;

    public UnityEvent<int,int> OnItemDropped;


    private ItemInfoView m_infoView;

    private void OnEnable()
    {
        OnItemClick.AddListener(OnItemClicked);
    }

    public bool ShowInfo = true;

    private void OnItemClicked(int itemId)
    {
        OnItemSelect?.Invoke(itemId);
        if (!ShowInfo)
            return;
        
        if (itemId != -1)
        {
            if (m_infoView == null)
            {
                m_infoView = Instantiate(m_itemInfoView, transform.parent);
            }

            m_infoView.ShowItemInfo(ItemRegistry.GetItem(itemId));
        }
        else
        {
            if (m_infoView != null)
            {
                Destroy(m_infoView.gameObject);
                m_infoView = null;
            }
        }
    }

    private void OnDisable()
    {
        OnItemClick.RemoveListener(OnItemClicked);
        for (int i = 0; i < m_itemStackViewParent.childCount; i++)
            Destroy(m_itemStackViewParent.GetChild(i).gameObject);
    }

    public void ShowLocalPlayerInventory()
    {
        Show(InventoryNetworkController.LocalInventory);
    }

    public void Show(ItemStack[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (i is NetworkInventory.HELMET_SLOT_INDEX
                or NetworkInventory.ARMOR_SLOT_INDEX or NetworkInventory.HAND_SLOT_INDEX)
                continue;

            var item = items[i];
            ItemStackView stackView = Instantiate(m_itemStackTemplate, m_itemStackViewParent);
            stackView.Id = i;

            if (item==null || item.Id == -1)
            {
                continue;
            }

            var materialData = ItemRegistry.GetItem(items[i].Id);
            var sprite = m_spriteDatabase.LoadSprite(items[i].Id);

            var contentView = Instantiate(m_itemStackContentView, stackView.Content);
            contentView.SetContext(sprite, item.Count, materialData.Stackable);
            contentView.ItemId = item.Id;
        }
    }

    public void Show(NetworkInventory networkInventory)
    {
        Show(networkInventory.Items);
    }
}