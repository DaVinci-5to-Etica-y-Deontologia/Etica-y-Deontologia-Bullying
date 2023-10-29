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

    private void OnTransformChildrenChanged()
    {
        contain.enabled = false;

        prevValue = containScrollRect.normalizedPosition.y;
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
