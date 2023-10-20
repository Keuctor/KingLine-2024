using UnityEngine;

public class NavigationUI : MonoBehaviour
{
    public void ShowInventory()
    {
        var inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController.IsVisible)
            inventoryController.HideInventory();
        else
            inventoryController.ShowInventory();
    }
}