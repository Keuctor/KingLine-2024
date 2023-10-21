
using System;
using DG.Tweening;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Item Popup")]
    [SerializeField]
    private ItemPopupUI m_itemPopup;

    [SerializeField]
    private Transform m_itemPopupContent;

    private void Start()
    {
        var controller = NetworkManager.Instance.GetController<InventoryNetworkController>();
        controller.OnAddItem.AddListener(ShowItemAddPopup);
    }
    
    public void ShowItemAddPopup(int id, int count,int total)
    {
        var itemInfo = ItemRegistry.GetItem(id);
        
        var popup = Instantiate(m_itemPopup, m_itemPopupContent);
        popup.Icon.sprite = SpriteLoader.LoadSprite(itemInfo.Name);
        popup.CountText.text = $"<size=60> +</size>{count} <color=\"white\"><size=70>({total})</size></color>";
        
        
        popup.CanvasGroup.alpha = 0;
        popup.RectTransform.anchoredPosition = new Vector2(0, 0);
        popup.RectTransform.DOAnchorPos(new Vector2(0, 200), 0.2f);
        popup.CanvasGroup.DOFade(1, 0.2f);
        popup.RectTransform.DOAnchorPosY(400, 0.2f).SetDelay(1);
        popup.CanvasGroup.DOFade(0, 0.2f).SetDelay(1);

        Destroy(popup.gameObject, 2f);
    }
}
