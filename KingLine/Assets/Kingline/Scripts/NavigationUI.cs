using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationUI : MonoBehaviour
{
    public void ShowInventory()
    {
        var inventoryController = FindObjectOfType<InventoryNetworkController>();
        if (inventoryController.IsVisible)
        {
            inventoryController.HideInventory();
        }
        else
        {
            inventoryController.ShowInventory();
        }
    }
}