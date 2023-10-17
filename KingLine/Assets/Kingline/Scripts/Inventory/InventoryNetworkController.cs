using System;
using System.Linq;
using Assets.HeroEditor.FantasyInventory.Scripts.Interface.Elements;
using DG.Tweening;
using UnityEngine;

public class InventoryNetworkController : NetworkController<InventoryNetworkController>
{
    [SerializeField]
    private GameObject m_inventoryView;
    

    [SerializeField]
    private ItemStackView m_itemViewTemplate;

    [SerializeField]
    private ItemStackView[] m_gearSets;

    [SerializeField]
    private ItemStackContentView m_itemViewContentTemplate;

    [SerializeField]
    private Transform m_itemViewContent;

    public ItemStack[] Items = new ItemStack[28];


    private bool m_shown;

    public bool IsVisible => m_shown;

    
    public ItemRegistry ItemRegistry = new ItemRegistry();

    [Header("Item Popup")]
    [SerializeField]
    private ItemPopupUI m_itemPopup;

    [SerializeField]
    private Transform m_itemPopupContent;



    private void OnInventoryMove(ResInventoryMove obj)
    {
        Items[obj.ToIndex] = Items[obj.FromIndex];
        Items[obj.FromIndex] = new ItemStack()
        {
            Count = 0,
            Id = -1,
        };

        if (obj.ToIndex >= 25 || obj.FromIndex >= 25)
        {
            OnGearChange?.Invoke();
        }
    }

    public Action OnGearChange;

    public void ShowInventory()
    {
        if (m_shown) return;

        m_shown = true;
        m_inventoryView.gameObject.SetActive(true);
        for (var i = 0; i < Items.Length; i++)
        {
            var m = Items[i];
            if (i >= 25)
            {
                var gearView = m_gearSets[Mathf.Abs(25 - i)];
                gearView.Id = i;
                var gearItem = ItemRegistry.GetItem(m.Id);
                if (gearItem != null)
                {
                    var contentView = Instantiate(m_itemViewContentTemplate, gearView.Content);
                    contentView.SetContext(SpriteLoader.LoadSprite(gearItem.Name), m.Count);
                }
                continue;
            }
            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.Id = i;
            var item = ItemRegistry.GetItem(m.Id);
            if (item != null)
            {
                var contentView = Instantiate(m_itemViewContentTemplate, view.Content);
                contentView.SetContext(SpriteLoader.LoadSprite(item.Name), m.Count);
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

        for (int i = 0; i < m_gearSets.Length; i++)
        {
            if (m_gearSets[i].transform.childCount > 0)
            {
                Destroy(m_gearSets[i].transform.GetChild(0).gameObject);
            }
        }
    }

    private void OnInventoryResult(ResInventory result)
    {
        Items = result.Items;
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


    public override void UnSubscribeResponse()
    {
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResInventory>();
        NetworkManager.Instance.NetPacketProcessor
            .RemoveSubscription<ResInventoryMove>();
    }

    private void OnInventoryAdd(ResInventoryAdd response)
    {
        bool found = false;
        for (var i = 0; i < Items.Length - 3; i++)
        {
            var item = Items[i];
            if (item.Id == response.Id)
            {
                item.Count += response.Count;
                found = true;
                break;
            }
        }

        if (!found)
        {
            for (var i = 0; i < Items.Length - 3; i++)
            {
                if (Items[i].Id == -1)
                {
                    Items[i].Id = response.Id;
                    Items[i].Count = response.Count;
                    break;
                }
            }
        }

        var itemInfo = ItemRegistry.GetItem(response.Id);

        var popup = Instantiate(m_itemPopup, m_itemPopupContent);
        popup.Icon.sprite = SpriteLoader.LoadSprite(itemInfo.Name);
        popup.CountText.text = "+" + response.Count;
        popup.CanvasGroup.alpha = 0;
        popup.RectTransform.anchoredPosition = new Vector2(0, 0);
        popup.RectTransform.DOAnchorPos(new Vector2(0, 200), 0.2f);
        popup.CanvasGroup.DOFade(1, 0.2f);
        popup.RectTransform.DOAnchorPosY(400, 0.2f).SetDelay(1);
        popup.CanvasGroup.DOFade(0, 0.2f).SetDelay(1);

        Destroy(popup.gameObject, 2f);
    }


    public override void OnDisconnectedFromServer()
    {
    }
}