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

    [SerializeField]
    public Image m_background;

    private Color _backgroundColor;

    [NonSerialized]
    public Transform ParentAfterDrag;

    public int ItemId;

    public void OnBeginDrag(PointerEventData eventData)
    {
        ItemStackView.From = transform.parent.GetComponent<ItemStackView>().Id;
        ParentAfterDrag = transform.parent;
        transform.SetParent(ParentAfterDrag.parent.parent.parent);
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

    public void SetCount(bool isStackable, int count)
    {
        if (isStackable)
            m_countText.text = "x" + count;
        else
            m_countText.text = "";
    }

    public void SetContext(Sprite icon, int count, bool isStackable)
    {
        m_image.sprite = icon;
        SetCount(isStackable, count);
    }
}