using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ItemSelectPopup : MonoBehaviour
{
    [SerializeField]
    private SelectionItemStackView m_itemSelectionViewTemplate;

    [SerializeField]
    private Transform m_parent;

    [SerializeField]
    private SelectionItemStackViewContent m_itemSelectionViewContent;

    public UnityEvent<int> OnSelect = new();

    public bool SelectMode = true;

    [SerializeField]
    private ItemInfoView m_itemInfoView;

    private NetworkInventory m_inventory;

    [SerializeField]
    private InventoryNetworkController m_inventoryNetworkController;

    [FormerlySerializedAs("m_selectAmountPopupTemplate")]
    [SerializeField]
    public SelectAmountView MSelectAmountViewTemplate;

    [FormerlySerializedAs("m_spriteLoader")]
    [SerializeField]
    private MaterialSpriteDatabase m_materialDatabase;
    
    private void OnEnable()
    {
        m_inventoryNetworkController.OnRemoveItem.AddListener(OnItemRemoved);
    }

    private void OnDisable()
    {
        m_inventoryNetworkController.OnRemoveItem.RemoveListener(OnItemRemoved);
    }

    private SelectionItemStackView[] views;

    private  void Start()
    {
        m_inventory = InventoryNetworkController.LocalInventory;

        var items = m_inventory.Items;
        views = new SelectionItemStackView[25];
        for (var i = 0; i < 25; i++)
        {
            var m = items[i];

            views[i] = Instantiate(m_itemSelectionViewTemplate, m_parent);
            views[i].OnClick.AddListener(OnClick);
            views[i].Id = i;

            if (m.Id != -1)
            {
                var item = ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemSelectionViewContent, views[i].Content);
                contentView.SetContext(m_materialDatabase.LoadSprite(item.Id), m.Count,
                    item.Stackable);
            }
        }
    }

    private void OnItemRemoved(int index, int newCount)
    {
        var selectionItemStackViewContent =
            views[index].Content.GetComponentInChildren<SelectionItemStackViewContent>();
        if (newCount <= 0)
        {
            Destroy(selectionItemStackViewContent.gameObject);
            m_itemInfoView.CanvasGroup.DOFade(0, 0.2f);
            m_itemInfoView.CanvasGroup.blocksRaycasts = false;
            return;
        }
        var items = InventoryNetworkController.LocalInventory.Items;
        var info = ItemRegistry.GetItem(items[index].Id);
        selectionItemStackViewContent.SetContext(
            m_materialDatabase.LoadSprite(info.Id), newCount, info.Stackable);
    }


    private void OnClick(int id)
    {
        if (SelectMode)
        {
            OnSelect?.Invoke(id);
            Destroy(gameObject);
        }
        else
        {
            var item = m_inventory.Items[id];
            if (item.Id != -1)
            {
                var n = ItemRegistry.GetItem(item.Id);
                m_itemInfoView.ShowItemInfo(n);
                m_itemInfoView.OnSellButtonClicked.RemoveAllListeners();
                var index = id;
                m_itemInfoView.OnSellButtonClicked.AddListener(() =>
                {
                    var selectPopup = Instantiate(MSelectAmountViewTemplate);
                    selectPopup.SetIcon(m_materialDatabase.LoadSprite(n.Id));
                    selectPopup.SetValue(1, 1, item.Count);
                    selectPopup.OnDone.AddListener(() =>
                    {
                        InventoryNetworkController.Sell(index, selectPopup.Value);
                        Destroy(selectPopup.gameObject);
                    });
                });
                if (m_itemInfoView.CanvasGroup.alpha == 0)
                {
                    m_itemInfoView.transform.localScale = Vector3.one * 1.1f;
                }

                m_itemInfoView.transform.DOScale(Vector3.one, 0.2f);
                m_itemInfoView.CanvasGroup.DOFade(1, 0.2f);
                m_itemInfoView.CanvasGroup.blocksRaycasts = true;
            }
            else
            {
                m_itemInfoView.CanvasGroup.DOFade(0, 0.2f);
                m_itemInfoView.CanvasGroup.blocksRaycasts = false;
            }
        }
    }
}