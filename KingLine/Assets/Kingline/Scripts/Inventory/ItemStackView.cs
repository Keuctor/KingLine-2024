using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragInfo
{
    public int Index;
    public Transform ItemContent;
}

public class ItemStackView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private Image m_background;

    private int m_index => transform.GetSiblingIndex() - 1;
    private static ItemStackView m_selectedItemStackView;

    [SerializeField]
    private Image m_image;

    [SerializeField]
    private TMP_Text m_count;

    [SerializeField]
    private Transform m_content;

    private static DragInfo m_dragInfo = new();

    private Image m_dragItem;
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        m_dragInfo.Index = this.m_index;
        m_dragInfo.ItemContent = this.m_content;
        this.m_content.gameObject.SetActive(false);
        if (m_dragItem == null)
        {
            m_dragItem =FindObjectOfType<InventoryNetworkController>().DragImage; 
            m_dragItem.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_dragItem.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_dragInfo.ItemContent.gameObject.SetActive(true);
        m_dragItem.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_selectedItemStackView != this)
            m_background.color = new Color(1, 0.9f, 0.8f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (m_selectedItemStackView != this)
            m_background.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_selectedItemStackView != null)
        {
            m_selectedItemStackView.m_background.color = Color.white;
        }

        m_selectedItemStackView = this;
        m_background.color = new Color(1, 0.7f, 0.5f, 1);
    }
}