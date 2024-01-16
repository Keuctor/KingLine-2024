using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemInfoView : MonoBehaviour
{
    [SerializeField] private ItemInfoMetaView m_metaViewTemplate;

    [SerializeField] private Transform m_metaViewParent;

    [SerializeField] private ItemInfoMetaView m_priceMetaView;

    [SerializeField] private TMP_Text m_itemName;

    [SerializeField] private Image m_itemIcon;

    [FormerlySerializedAs("m_canvasGroup")] [SerializeField]
    public CanvasGroup CanvasGroup;

    [SerializeField] private Transform m_buttonContainer;

    [SerializeField] private Button m_buttonPrefab;

    private readonly List<ItemInfoMetaView> _metaViews = new(4);

    [SerializeField] private MaterialSpriteDatabase m_materialDatabase;

    public void ClearButtons()
    {
        for(int i=0;i<m_buttonContainer.childCount;i++)
            Destroy(m_buttonContainer.GetChild(i).gameObject);
    }
    public void AddButton(string text, Action action)
    {
        var btn = Instantiate(m_buttonPrefab, m_buttonContainer);
        btn.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        btn.onClick.AddListener(() => { action?.Invoke();});
    }

    public Transform ShowItemInfo(IItemMaterial itemMaterial)
    {
        m_itemIcon.sprite = m_materialDatabase.LoadSprite(itemMaterial.Id);
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
        m_buttonContainer.transform.SetAsLastSibling();
        return transform;
    }
}