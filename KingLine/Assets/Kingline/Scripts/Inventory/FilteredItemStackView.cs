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
        if (Id == From)
            return;
        var dropped = eventData.pointerDrag;
        var view = dropped.GetComponent<ItemStackContentView>();
        if (view == null) return;
        var inv = InventoryNetworkController.LocalInventory;
        var item = inv.Items[From];
        var itemInfo = ItemRegistry.GetItem(item.Id);

        if (Id == NetworkInventory.HAND_SLOT_INDEX)
        {
            if (itemInfo.Type == IType.WEAPON)
            {
                DragItem(view, eventData);
            }
        }

        if (Id == NetworkInventory.HELMET_SLOT_INDEX)
        {
            if (itemInfo.Type == IType.HELMET)
            {
                DragItem(view, eventData);
            }
        }

        if (Id == NetworkInventory.ARMOR_SLOT_INDEX)
        {
            if (itemInfo.Type == IType.ARMOR)
            {
                DragItem(view, eventData);
            }
        }
    }

    public void DragItem(ItemStackContentView view, PointerEventData eventData)
    {
        if (Content.childCount != 0) return;
        view.ParentAfterDrag = transform;
        To = this.Id;
        OnPointerDown(eventData);
        NetworkManager.Instance.Send(new ReqInventoryMove()
        {
            FromIndex = (short)From,
            ToIndex = (short)To
        });
    }
}