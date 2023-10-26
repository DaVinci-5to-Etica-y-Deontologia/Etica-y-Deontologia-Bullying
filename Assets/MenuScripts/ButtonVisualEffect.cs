using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonVisualEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleAmount;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale += new Vector3(scaleAmount, scaleAmount, scaleAmount);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale -= new Vector3(scaleAmount, scaleAmount, scaleAmount);
    }
}
