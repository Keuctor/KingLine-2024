using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryNetworkController : NetworkController
{
    [SerializeField]
    private GameObject m_inventoryView;

    [SerializeField]
    private ItemStackView m_itemViewTemplate;

    [SerializeField]
    private ItemStackContentView m_itemViewContentTemplate;

    [SerializeField]
    private Transform m_itemViewContent;

    private ItemStack[] m_items = new ItemStack[25];

    private bool m_shown;

    public bool IsVisible => m_shown;

    [SerializeField]
    public ItemsSO m_itemInfo;

    private static InventoryNetworkController m_controller;

    [Header("Item Popup")]
    [SerializeField]
    private ItemPopupUI m_itemPopup;

    [SerializeField]
    private Transform m_itemPopupContent;

    private void Awake()
    {
        if (m_controller != null)
        {
            if (m_controller != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        m_controller = this;
        DontDestroyOnLoad(gameObject);
    }


    private void OnInventoryMove(ResInventoryMove obj)
    {
        m_items[obj.ToIndex] = m_items[obj.FromIndex];
        m_items[obj.FromIndex] = new ItemStack()
        {
            Count = 0,
            Id = -1,
        };
    }

    public void ShowInventory()
    {
        if (m_shown) return;

        m_shown = true;
        m_inventoryView.gameObject.SetActive(true);
        int index = 0;
        foreach (var m in m_items)
        {
            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.Id = index++;
            var item = m_itemInfo.GetItem(m.Id);
            if (item != null)
            {
                var contentView = Instantiate(m_itemViewContentTemplate, view.Content);
                contentView.SetContext(item.Icon, m.Count);
            }
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
        for (int i = 0; i < m_itemViewContent.transform.childCount; i++)
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
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResInventoryMove>(OnInventoryMove);
        NetworkManager.Instance.NetPacketProcessor
            .SubscribeReusable<ResInventoryAdd>(OnInventoryAdd);
    }

    private void OnInventoryAdd(ResInventoryAdd response)
    {
        bool found = false;
        for (var i = 0; i < m_items.Length; i++)
        {
            var item = m_items[i];
            if (item.Id == response.Id)
            {
                item.Count += response.Count;
                found = true;
                break;
            }
        }

        if (!found)
        {
            for (var i = 0; i < m_items.Length; i++)
            {
                if (m_items[i].Id == -1)
                {
                    m_items[i].Id = response.Id;
                    m_items[i].Count = response.Count;
                    break;
                }
            }
        }

        var itemInfo = m_itemInfo.GetItem(response.Id);

        var popup = Instantiate(m_itemPopup, m_itemPopupContent);
        popup.Icon.sprite = itemInfo.Icon;
        popup.CountText.text = "+" + response.Count;
        popup.CanvasGroup.alpha = 0;
        popup.RectTransform.anchoredPosition = new Vector2(0, 0);
        popup.RectTransform.DOAnchorPos(new Vector2(0, 200), 0.2f);
        popup.CanvasGroup.DOFade(1, 0.2f);
        popup.RectTransform.DOAnchorPosY(400, 0.2f).SetDelay(1);
        popup.CanvasGroup.DOFade(0, 0.2f).SetDelay(1);
    }


    public override void UnSubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResInventory>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResInventoryMove>();
    }

    public override void OnDisconnectedFromServer()
    {
    }
}