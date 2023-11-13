using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellItemView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField]
    private ItemInfoView m_itemInfoView;

    [SerializeField]
    private InventoryNetworkController m_inventoryNetworkController;

    [Header("UI")]
    [SerializeField]
    private Button m_sellButton;

    private TMP_Text m_sellButtonText;


    [SerializeField]
    private GameObject m_notSelectedAnyItemPanel;

    private void Start()
    {
        m_sellButtonText = m_sellButton.transform.GetChild(0).GetComponent<TMP_Text>();
        m_notSelectedAnyItemPanel.gameObject.SetActive(true);
        m_inventoryNetworkController.OnRemoveItem.AddListener(OnRemoveItem);
        this.m_sellButton.onClick.AddListener(OnSellItemClicked);
    }

    private void OnRemoveItem(int index, int count)
    {
        if (selectedIndex == index)
        {
            if (count == 0)
            {
                m_notSelectedAnyItemPanel.gameObject.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        this.m_sellButton.onClick.RemoveListener(OnSellItemClicked);
        m_inventoryNetworkController.OnRemoveItem.AddListener(OnRemoveItem);
    }

    public int selectedIndex = -1;

    public void OnSellItemClicked()
    {
        var invItem = InventoryNetworkController.LocalInventory.Items[selectedIndex];
        if (invItem.Count == 1)
        {
            InventoryNetworkController.Sell(selectedIndex, 1);
            return;
        }

        var p = PopupManager.Instance.CreateNew("SelectAmount");
        var selectAmountView = p.Add(PopupManager.Instance.SelectAmountView);
        selectAmountView.SetValue(1, 1, invItem.Count);


        selectAmountView.OnDone.AddListener((bool done) =>
        {
            if (done)
            {
                InventoryNetworkController.Sell(selectedIndex, selectAmountView.Value);
            }

            p.Destroy();
        });
    }

    public void SetItemId(int index)
    {
        selectedIndex = index;
        var invItem = InventoryNetworkController.LocalInventory.Items[index];

        var itemId = invItem.Id;
        m_notSelectedAnyItemPanel.gameObject.SetActive(itemId == -1);
        if (itemId == -1)
            return;

        var item = ItemRegistry.GetItem(itemId);
        this.m_itemInfoView.ShowItemInfo(item);

        m_sellButtonText.text = $"Sell ({item.Value})";
    }
}