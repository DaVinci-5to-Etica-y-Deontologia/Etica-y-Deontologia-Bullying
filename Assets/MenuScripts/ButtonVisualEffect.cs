using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class ButtonVisualEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 scaleAmount;
    [SerializeField] private Ease easeType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!GetComponent<Button>().interactable) return;
        transform.DOScale(scaleAmount, 0.1f).SetEase(easeType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.1f).SetEase(easeType);
    }
}
