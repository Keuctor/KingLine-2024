using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectionItemStackView : MonoBehaviour,
    IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public Transform Content => transform;
    private Vector3 initialScale; 
    private void Start()
    {
        initialScale = transform.localScale;
    }

    public int Id;
    public UnityEvent<int> OnClick = new();

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(initialScale * 1.2f, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initialScale, 0.1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(Id);
    }
}
