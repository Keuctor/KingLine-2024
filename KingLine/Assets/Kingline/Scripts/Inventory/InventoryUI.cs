using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    public static UnityEvent<int> OnItemClick = new();

    [SerializeField]
    private ItemStackView m_itemViewTemplate;

    [SerializeField]
    private ItemStackView[] m_gearSets;

    [SerializeField]
    private ItemStackContentView m_itemViewContentTemplate;

    [SerializeField]
    private Transform m_itemViewContent;

    public TMP_Text TotalStrengthText;
    public TMP_Text TotalArmorText;
    public TMP_Text CoinText;

    [SerializeField]
    private ItemInfoView m_itemInfoView;

    private ProgressionNetworkController m_progressionNetworkController;

    public InventoryNetworkController m_inventoryNetworkController;

    public CharacterTextureView m_characterTextureView;

    private void Start()
    {
        m_inventoryNetworkController.OnGearChange.AddListener(OnGearsetChanged);
        OnItemClick.AddListener(OnItemClicked);
    }

    private void OnEnable()
    {
        ShowInventory();
    }

    private void OnDisable()
    {
        ClearInventoryUI();
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

    private void OnGearsetChanged(int id)
    {
        if (id == NetworkManager.LocalPlayerPeerId)
            DisplayGear();
    }

 

    public void ShowInventory()
    {
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
                    contentView.SetContext(MenuController.Instance.SpriteLoader.LoadSprite(m.Id), m.Count, gearItem.Stackable);
                }
                continue;
            }

            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.Id = i;
            if (m.Id != -1)
            {
                var item = ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemViewContentTemplate, view.Content);
                contentView.SetContext(MenuController.Instance.SpriteLoader.LoadSprite(m.Id), m.Count, item.Stackable);
            }
        }
        DisplayGear();
    }

  
    private void ClearInventoryUI()
    {
        
        for (var i = 0; i < m_itemViewContent.transform.childCount; i++)
            Destroy(m_itemViewContent.transform.GetChild(i).gameObject);

        for (var i = 0; i < m_gearSets.Length; i++)
            if (m_gearSets[i].transform.childCount > 0)
                Destroy(m_gearSets[i].transform.GetChild(0).gameObject);
    }

    private void DisplayGear()
    {
        var inventory = InventoryNetworkController.LocalInventory.GetGear();
        
        m_characterTextureView.Show(inventory);
        
        var baseStrength = m_progressionNetworkController.GetSkill("Strength");
        var baseDefence = m_progressionNetworkController.GetSkill("Defence");
        
        TotalArmorText.text = m_characterTextureView.Armor+ baseDefence + "";
        TotalStrengthText.text = m_characterTextureView.Strength + baseStrength + "";
        CoinText.text = PlayerNetworkController.LocalPlayer.Currency + "";
    }
}