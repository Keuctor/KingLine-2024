using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopupUI : MonoBehaviour
{
    public TMP_Text CountText;
    public Image Icon;
    public CanvasGroup CanvasGroup;

    private RectTransform m_rectTransform;

    public RectTransform RectTransform
    {
        get
        {
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();
            return m_rectTransform;
        }
    }
}