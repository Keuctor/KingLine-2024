using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemInfoView : MonoBehaviour
{
    [SerializeField]
    private ItemInfoMetaView m_metaViewTemplate;

    [SerializeField]
    private Transform m_metaViewParent;

    [SerializeField]
    private ItemInfoMetaView m_priceMetaView;

    [SerializeField]
    private TMP_Text m_itemName;

    [SerializeField]
    private Image m_itemIcon;
    [FormerlySerializedAs("m_canvasGroup")]
    [SerializeField]
    public CanvasGroup CanvasGroup;

    private readonly List<ItemInfoMetaView> _metaViews = new(4);

    [SerializeField]
    private Button m_sellButton;

    public UnityEvent OnSellButtonClicked = new();

    [SerializeField]
    private SpriteLoader m_spriteLoader;
    

    public void ShowItemInfo(IItemMaterial itemMaterial)
    {
        m_itemIcon.sprite = m_spriteLoader.LoadSprite(itemMaterial.Id);
        m_itemName.text = itemMaterial.Name;
        m_priceMetaView.MetaValue.text = "" + itemMaterial.Value;

        if(m_sellButton){
            m_sellButton.onClick.RemoveAllListeners();
            m_sellButton.onClick.AddListener(() =>
            {
                OnSellButtonClicked?.Invoke();
                
            });
        }

        for (var i = 0; i < _metaViews.Count; i++)
            Destroy(_metaViews[i].gameObject);
        _metaViews.Clear();

        switch (itemMaterial.Type)
        {
            case IType.HELMET:
            case IType.ARMOR:
            {
                var metaView = Instantiate(m_metaViewTemplate, m_metaViewParent);
                var armor = (ArmorItemMaterial)itemMaterial;
                metaView.MetaValue.text = "+" + armor.Armor;
                metaView.MetaName.text = "Armor";
                _metaViews.Add(metaView);
            }
                break;
            case IType.TOOL:
            {
                var metaView = Instantiate(m_metaViewTemplate, m_metaViewParent);
                var armor = (ToolItemMaterial)itemMaterial;
                metaView.MetaValue.text = "x" + armor.ToolValue;
                metaView.MetaName.text = "Mining Speed";
                _metaViews.Add(metaView);
            }
                break;
            case IType.RESOURCE:
                break;
            case IType.WEAPON:
            {
                var metaView = Instantiate(m_metaViewTemplate, m_metaViewParent);
                var armor = (WeaponItemMaterial)itemMaterial;
                metaView.MetaValue.text = "+" + armor.Attack;
                metaView.MetaName.text = "Damage";
                _metaViews.Add(metaView);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        m_priceMetaView.transform.SetAsLastSibling();
        if (m_sellButton)
        {
            m_sellButton.transform.SetAsLastSibling();
        }
    }
}