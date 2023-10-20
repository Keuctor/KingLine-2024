using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private readonly List<ItemInfoMetaView> _metaViews = new(4);

    public void ShowItemInfo(IItemMaterial itemMaterial)
    {
        m_itemIcon.sprite = SpriteLoader.LoadSprite(itemMaterial.Name);
        m_itemName.text = itemMaterial.Name;
        m_priceMetaView.MetaValue.text = "" + itemMaterial.Value;


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
    }
}