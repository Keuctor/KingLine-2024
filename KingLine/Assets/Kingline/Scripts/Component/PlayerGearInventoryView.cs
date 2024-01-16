using UnityEngine;

public class PlayerGearInventoryView : MonoBehaviour
{
    public ItemStackView[] Items;

    [Header("Dependency"), SerializeField]
    private MaterialSpriteDatabase m_spriteDatabase;

    [SerializeField]
    private InventoryNetworkController m_inventoryController;

    [Header("Prefab"), SerializeField]
    private ItemStackContentView m_itemStackContentView;

    public void DisplayGear(ItemStack[] gearInv)
    {
        for (int i = 0; i < gearInv.Length; i++)
        {
            if (gearInv[i].Id != -1)
            {
                var item = Instantiate(m_itemStackContentView, Items[i].Content);
                item.ItemId = gearInv[i].Id;
                item.SetContext(m_spriteDatabase.LoadSprite(gearInv[i].Id), 0, false);
            }
        }
    }
}