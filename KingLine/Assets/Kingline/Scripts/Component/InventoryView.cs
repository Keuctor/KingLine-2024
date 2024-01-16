using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryView : MonoBehaviour
{
    public static UnityEvent<int,int> OnItemClick = new();

    public UnityEvent<int> OnItemSelect = new();

    [Header("Dependency")] [SerializeField]
    private MaterialSpriteDatabase m_spriteDatabase;

    [Header("Prefabs")] [SerializeField] public ItemStackView m_itemStackTemplate;

    [SerializeField] private ItemInfoView m_itemInfoView;

    [SerializeField] private ItemStackContentView m_itemStackContentView;

    [Header("Parents")] [SerializeField] public Transform m_itemStackViewParent;

    private ItemInfoView m_infoView;
    public ulong InventoryId { get; set; }

    private Dictionary<int, ItemStackContentView> m_createdContentViews = new Dictionary<int, ItemStackContentView>();

    private void OnEnable()
    {
        OnItemClick.AddListener(OnItemClicked);
        //m_inventoryNetworkController.OnRemoveItem.AddListener(OnRemoveItem);
    }

    private void OnRemoveItem(int index, int newCount)
    {
        // if (newCount == 0)
        // {
        //     Destroy(m_createdContentViews[index].gameObject);
        //     m_createdContentViews.Remove(index);
        // }
        // else
        // {
        //     var item = InventoryNetworkController.LocalInventory.Items[index];
        //     var itemData = ItemRegistry.GetItem(item.Id);
        //     m_createdContentViews[index].SetCount(itemData.Stackable, newCount);
        // }
    }

    public bool ShowInfo = true;

    public ItemInfoView InfoView => m_infoView;
    private void OnItemClicked(int index,int itemId)
    {
        if (ShowInfo)
        {
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
        OnItemSelect?.Invoke(itemId);
    }

    private void OnDisable()
    {
        OnItemClick.RemoveListener(OnItemClicked);
        for (int i = 0; i < m_itemStackViewParent.childCount; i++)
            Destroy(m_itemStackViewParent.GetChild(i).gameObject);
    }

    public void Show(ulong id, ItemStack[] items)
    {
        this.InventoryId = id;

        m_createdContentViews.Clear();

        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            var stackView = Instantiate(m_itemStackTemplate, m_itemStackViewParent);
            stackView.Index = (ushort)i;

            if (item == null || item.Id == -1)
            {
                continue;
            }

            var materialData = ItemRegistry.GetItem(items[i].Id);
            var sprite = m_spriteDatabase.LoadSprite(items[i].Id);

            var contentView = Instantiate(m_itemStackContentView, stackView.Content);
            contentView.SetContext(sprite, item.Count, materialData.Stackable);
            contentView.ItemId = item.Id;
            m_createdContentViews.Add(i, contentView);
        }
    }

    public void Show(NetworkInventory networkInventory)
    {
        Show(networkInventory.Id, networkInventory.Items);
    }
}