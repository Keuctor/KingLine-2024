using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour
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

    [Header("Item Popup")]
    [SerializeField]
    private ItemPopupUI m_itemPopup;

    [SerializeField]
    private Transform m_itemPopupContent;

    public TMP_Text TotalStrengthText;
    public TMP_Text TotalArmorText;
    public TMP_Text CoinText;

    public bool IsVisible => m_shown;

    private bool m_shown;

    [SerializeField]
    private ItemInfoView m_itemInfoView;

    public static UnityEvent<int> OnItemClick = new();


    private void Start()
    {
        var controller = NetworkManager.Instance.GetController<InventoryNetworkController>();
        controller.OnGearChange.AddListener(OnGearsetChanged);
        controller.OnAddItem.AddListener(ShowItemAddPopup);
        OnItemClick.AddListener(OnItemClicked);
    }

    private void OnItemClicked(int index)
    {
        var item = InventoryNetworkController.LocalInventory.Items[index];
        if (item.Id == -1)
        {
            m_itemInfoView.gameObject.SetActive(false);
        }
        else
        {
            m_itemInfoView.gameObject.SetActive(true);
            m_itemInfoView.ShowItemInfo(ItemRegistry.GetItem(item.Id));
        }
    }

    void OnGearsetChanged(int id)
    {
        if (id == NetworkManager.LocalPlayerPeerId)
            DisplayGear();
    }

    public void ShowItemAddPopup(int id, int count)
    {
        var itemInfo = ItemRegistry.GetItem(id);

        var popup = Instantiate(m_itemPopup, m_itemPopupContent);
        popup.Icon.sprite = SpriteLoader.LoadSprite(itemInfo.Name);
        popup.CountText.text = "+" + count;
        popup.CanvasGroup.alpha = 0;
        popup.RectTransform.anchoredPosition = new Vector2(0, 0);
        popup.RectTransform.DOAnchorPos(new Vector2(0, 200), 0.2f);
        popup.CanvasGroup.DOFade(1, 0.2f);
        popup.RectTransform.DOAnchorPosY(400, 0.2f).SetDelay(1);
        popup.CanvasGroup.DOFade(0, 0.2f).SetDelay(1);

        Destroy(popup.gameObject, 2f);
    }

    public void ShowInventory()
    {
        if (m_shown) return;

        m_shown = true;
        m_inventoryView.gameObject.SetActive(true);
        var items = InventoryNetworkController.LocalInventory.Items;

        for (var i = 0; i < items.Length; i++)
        {
            var m = items[i];
            if (i >= 25)
            {
                var gearView = m_gearSets[Mathf.Abs(25 - i)];
                gearView.Id = i;
                if (m.Id != -1)
                {
                    var gearItem = ItemRegistry.GetItem(m.Id);
                    var contentView = Instantiate(m_itemViewContentTemplate, gearView.Content);
                    contentView.SetContext(SpriteLoader.LoadSprite(gearItem.Name), m.Count, gearItem.Stackable);
                }

                continue;
            }

            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.Id = i;
            if (m.Id != -1)
            {
                var item = ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemViewContentTemplate, view.Content);
                contentView.SetContext(SpriteLoader.LoadSprite(item.Name), m.Count, item.Stackable);
            }
        }

        DisplayGear();
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

    private async void DisplayGear()
    {
        var inventory = await InventoryNetworkController.GetInventoryAsync();
        var helmet = inventory.GetHelmet();
        var armor = inventory.GetArmor();
        var hand = inventory.GetHand();

        var m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();

        var baseStrength = m_progressionNetworkController.GetSkill("Strength");
        var baseDefence = m_progressionNetworkController.GetSkill("Defence");

        if (helmet.Id != -1)
        {
            var item = ItemRegistry.GetItem(helmet.Id);
            var armorMaterial = (ArmorItemMaterial)item;
            baseDefence += (byte)armorMaterial.Armor;
        }

        if (armor.Id != -1)
        {
            var item = ItemRegistry.GetItem(armor.Id);
            var armorMaterial = (ArmorItemMaterial)item;
            baseDefence += (byte)armorMaterial.Armor;
        }

        if (hand.Id != -1)
        {
            var item = ItemRegistry.GetItem(hand.Id);
            var armorMaterial = (WeaponItemMaterial)item;
            baseStrength += (byte)armorMaterial.Attack;
        }

        TotalArmorText.text = baseDefence + "";
        TotalStrengthText.text = baseStrength + "";
        CoinText.text = NetworkManager.Instance.GetController<PlayerNetworkController>().LocalPlayer.Coin + "";
    }
}