using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemStackView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,
    IPointerExitHandler, IDropHandler
{
    public static int From;

    public static int To;

    public static ItemStackView _selectedItemView;
    public Transform Content;

    [SerializeField]
    private Image m_background;

    public int Id;

    public Color SELECTED_BACKGROUND_COLOR = new(0.5f, 0.6f, 0.7f, 0.5f);
    public Color NOT_SELECTED_BACKGROUND_COLOR = new(0.4f, 0.4f, 0.4f, 1f);
    public Color POINTER_OVER_BACKGROUND_COLOR = new(0.5f, 0.5f, 0.5f, 1f);


    private void Start()
    {
        m_background.color = NOT_SELECTED_BACKGROUND_COLOR;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag;
        var view = dropped.GetComponent<ItemStackContentView>();
        if (view != null)
            if (Content.childCount == 0)
            {
                view.ParentAfterDrag = transform;
                To = Id;
                OnPointerDown(eventData);
                NetworkManager.Instance.Send(new ReqInventoryMove
                {
                    FromIndex = (short)From,
                    ToIndex = (short)To
                });
            }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_selectedItemView != null)
            _selectedItemView.m_background.color = _selectedItemView.NOT_SELECTED_BACKGROUND_COLOR;

        InventoryController.OnItemClick?.Invoke(Id);
        _selectedItemView = this;
        _selectedItemView.m_background.color = SELECTED_BACKGROUND_COLOR;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_selectedItemView != this) m_background.color = POINTER_OVER_BACKGROUND_COLOR;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_selectedItemView != this) m_background.color = NOT_SELECTED_BACKGROUND_COLOR;
    }
}