using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryNetworkController : NetworkController
{
    [SerializeField]
    private GameObject m_inventoryView;

    [SerializeField]
    private GameObject m_itemViewTemplate;

    [SerializeField]
    private Transform m_itemViewContent;

    private ItemStack[] m_items = new ItemStack[25];

    private bool m_shown;

    public Image DragImage;

    public void ShowInventory()
    {
        if (m_shown) return;

        m_shown = true;
        m_inventoryView.gameObject.SetActive(true);
        foreach (var m in m_items)
        {
            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.SetActive(true);
        }
    }

    private void Update()
    {
        if (m_shown)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                HideInventory();
        }
    }

    public void HideInventory()
    {
        m_inventoryView.gameObject.SetActive(false);
        m_shown = false;
        ClearInventoryUI();
    }

    private void ClearInventoryUI()
    {
        for (int i = 1; i < m_itemViewContent.transform.childCount; i++)
            Destroy(m_itemViewContent.transform.GetChild(i).gameObject);
    }

    private void OnInventoryResult(ResInventory result)
    {
        m_items = result.Items;
        if (m_shown)
            ShowInventory();
    }

    public override void HandleRequest()
    {
        NetworkManager.Instance.Send(new ReqInventory());
    }

    public override void SubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResInventory>(OnInventoryResult);
    }

    public override void UnSubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResInventory>();
    }

    public override void OnDisconnectedFromServer()
    {
    }
}