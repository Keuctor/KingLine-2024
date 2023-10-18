using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionItemStackViewContent : MonoBehaviour
{
    [SerializeField]
    private Image m_image;
    [SerializeField]
    private TMP_Text m_countText;

    public void SetContext(Sprite icon, int count,bool stackable)
    {
        this.m_image.sprite = icon;
        if (stackable)
        {
            this.m_countText.text = "x" + count;
        }
        else
        {
            this.m_countText.text = "";
        }
    }
}
