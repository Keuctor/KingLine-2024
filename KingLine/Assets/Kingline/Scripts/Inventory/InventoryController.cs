using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

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

    public InventoryNetworkController m_inventoryNetworkController;

    public ItemRegistry ItemRegistry = new ItemRegistry();

    private void Start()
    {
        m_inventoryNetworkController = NetworkManager.Instance.GetController<InventoryNetworkController>();
        m_inventoryNetworkController.OnGearChange.AddListener(OnGearsetChanged);
        m_inventoryNetworkController.OnAddItem.AddListener(ShowItemAddPopup);
    }

    void OnGearsetChanged()
    {
        DisplayGearsetValues();
    }

    public void ShowItemAddPopup(int id,int count)
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
        for (var i = 0; i < m_inventoryNetworkController.Items.Length; i++)
        {
            var m = m_inventoryNetworkController.Items[i];
            if (i >= 25)
            {
                var gearView = m_gearSets[Mathf.Abs(25 - i)];
                gearView.Id = i;
                if (m.Id != -1)
                {
                    var gearItem = ItemRegistry.GetItem(m.Id);
                    var contentView = Instantiate(m_itemViewContentTemplate, gearView.Content);
                    contentView.SetContext(SpriteLoader.LoadSprite(gearItem.Name), m.Count);
                }

                continue;
            }

            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.Id = i;
            if (m.Id != -1)
            {
                var item = ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemViewContentTemplate, view.Content);
                contentView.SetContext(SpriteLoader.LoadSprite(item.Name), m.Count);
            }
        }

        DisplayGearsetValues();
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
    
    private void DisplayGearsetValues()
    {
        var helmet = m_inventoryNetworkController.Items[25].Id;
        var chest = m_inventoryNetworkController.Items[26].Id;
        var hand = m_inventoryNetworkController.Items[27].Id;

        var m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();

        var baseStrength = m_progressionNetworkController.GetSkill("Strength");
        var baseDefence = m_progressionNetworkController.GetSkill("Defence");

        if (helmet != -1)
        {
            var item = ItemRegistry.GetItem(helmet);
            var armorMaterial = (ArmorItemMaterial)item;
            baseDefence += (byte)armorMaterial.Armor;
        }

        if (chest != -1)
        {
            var item = ItemRegistry.GetItem(chest);
            var armorMaterial = (ArmorItemMaterial)item;
            baseDefence += (byte)armorMaterial.Armor;
        }

        if (hand != -1)
        {
            var item = ItemRegistry.GetItem(hand);
            var armorMaterial = (WeaponItemMaterial)item;
            baseStrength += (byte)armorMaterial.Attack;
        }

        TotalArmorText.text = baseDefence + "";
        TotalStrengthText.text = baseStrength + "";
        CoinText.text = NetworkManager.Instance.GetController<PlayerNetworkController>().LocalPlayer.Coin + "";
    }

}
