using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectionItemStackViewContent : MonoBehaviour
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

}
