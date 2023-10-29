using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatContentRefresh : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.ContentSizeFitter contain;

    private void OnTransformChildrenChanged()
    {
        contain.enabled = false;
    }
}
