using UnityEngine;
using UnityEngine.Events;

public class ItemSelectPopup : MonoBehaviour
{
    [SerializeField]
    private SelectionItemStackView m_itemSelectionViewTemplate;

    [SerializeField]
    private Transform m_parent;

    [SerializeField]
    private SelectionItemStackViewContent m_itemSelectionViewContent;

    public UnityEvent<int> OnSelect = new();

    private async void Start()
    {
        var inventory = await InventoryNetworkController.GetInventoryAsync();
        var items = inventory.Items;
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
        OnSelect?.Invoke(id);
        Destroy(gameObject);
    }
}