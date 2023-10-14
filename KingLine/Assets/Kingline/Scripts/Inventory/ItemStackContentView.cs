using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemStackContentView : MonoBehaviour, IBeginDragHandler, IEndDragHandler,
    IDragHandler
{
    [SerializeField]
    private Image m_image;
    [SerializeField]
    private TMP_Text m_countText;

    public void SetContext(Sprite icon, int count)
    {
        this.m_image.sprite = icon;
        this.m_countText.text = "x"+count;
    }

    [SerializeField]
    public Image m_background;
    
    [NonSerialized]
    public Transform ParentAfterDrag;

    private Color _backgroundColor;

    public void OnBeginDrag(PointerEventData eventData)
    {
        ItemStackView.From = transform.parent.GetComponent<ItemStackView>().Id;
        ParentAfterDrag = transform.parent;
        transform.SetParent(ParentAfterDrag.parent.parent);
        transform.SetAsLastSibling();
        m_background.raycastTarget = false;
        _backgroundColor = m_background.color;
        m_background.color = Color.clear;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(ParentAfterDrag);
        m_background.raycastTarget = true;
        m_background.color = _backgroundColor;
        var rect = GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = Vector2.zero;
    }
}