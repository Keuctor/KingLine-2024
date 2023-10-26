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
    
    private async void Start()
    {
        MenuController.Instance.Menu.Enqueue(this.gameObject);
        m_inventory = await InventoryNetworkController.GetInventoryAsync();
        var items = m_inventory.Items;
        for (var i = 0; i < 25; i++)
        {
            var m = items[i];

            var view = Instantiate(m_itemSelectionViewTemplate, m_parent);
            view.OnClick.AddListener(OnClick);
            view.Id = i;

            if (m.Id != -1)
            {
                var item = ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemSelectionViewContent, view.Content);
                contentView.SetContext(MenuController.Instance.SpriteLoader.LoadSprite(item.Id), m.Count, item.Stackable);
            }
        }
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
                m_itemInfoView.OnSellButtonClicked.AddListener(() =>
                {
                    Debug.Log("sell :" + item.Id);
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