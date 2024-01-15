using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemStackContentView : MonoBehaviour
{
    [SerializeField]
    private Image m_image;

    [SerializeField]
    private TMP_Text m_countText;

    private Color _backgroundColor;

    public int ItemId;

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