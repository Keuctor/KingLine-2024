

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FilteredItemStackView : ItemStackView
{
    private void Awake()
    {
        SELECTED_BACKGROUND_COLOR = Color.green;
        NOT_SELECTED_BACKGROUND_COLOR = Color.white;
        POINTER_OVER_BACKGROUND_COLOR = Color.white;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        ItemStackContentView view = dropped.GetComponent<ItemStackContentView>();
        if (view != null)
        {
            var item = InventoryNetworkController.Instance.Items[ItemStackView.From];
            var itemInfo = InventoryNetworkController.Instance.ItemRegistry.GetItem(item.Id);
            if (Id == 25)
            {
                if (itemInfo.Type.Equals("Helmet"))
                {
                    DragItem(view, eventData);
                }
            }
            else if (Id == 26)
            {
                if (itemInfo.Type.Equals("Armor"))
                {
                    DragItem(view,eventData);
                }
            }
            else if (Id == 27)
            {
                if (itemInfo.Type.Equals("Weapon") || itemInfo.Type.Equals("Pickaxe"))
                {
                    DragItem(view,eventData);
                }
            }
        }
    }

    public void DragItem(ItemStackContentView view,PointerEventData eventData)
    {
        if (Content.childCount == 0)
        {
            view.ParentAfterDrag = transform;
            ItemStackView.To = this.Id;
            OnPointerDown(eventData);
                
            NetworkManager.Instance.Send(new ReqInventoryMove()
            {
                FromIndex = (short)From,
                ToIndex = (short)To
            });
        }
    }
}
