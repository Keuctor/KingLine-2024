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

    private void Start()
    {
        var controller = NetworkManager.Instance.GetController<InventoryNetworkController>();
        var items =  controller.Items;
        for (var i = 0; i < 25; i++)
        {
            var m = items[i];

            var view = Instantiate(m_itemSelectionViewTemplate, m_parent);
            view.OnClick.AddListener(OnClick);
            view.Id = i;
            
            if (m.Id != -1)
            {
                var item =  FindObjectOfType<InventoryController>().ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemSelectionViewContent, view.Content);
                contentView.SetContext(SpriteLoader.LoadSprite(item.Name), m.Count);
            }
        }
    }

    private void OnClick(int id)
    {
        OnSelect?.Invoke(id);
        Destroy(gameObject);
    }
}