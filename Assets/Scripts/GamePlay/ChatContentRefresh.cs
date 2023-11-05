using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatContentRefresh : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    [SerializeField]
    UnityEngine.UI.Scrollbar containScroll;

    [SerializeField]
    UnityEngine.UI.ScrollRect containScrollRect;

    float prevValue;

    int childCount;

    private void OnTransformChildrenChanged()
    {
        if(childCount < transform.childCount)
            contain.enabled = false;

        prevValue = containScrollRect.normalizedPosition.y;

        childCount = transform.childCount;
    }

    public void OnValueChange(Vector2 value)
    {
        if (prevValue < 0.05f)
        {
            containScrollRect.normalizedPosition = Vector2.zero;
            containScroll.value = 0;
        }

        prevValue = value.y;
    }
}
