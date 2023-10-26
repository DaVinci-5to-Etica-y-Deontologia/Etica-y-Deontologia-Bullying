using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventCall : MonoBehaviour
{
    [SerializeField]
    UnityEvent<Button> eventToCall;

    [SerializeField]
    Button button;

    public void OnClick()
    {
        eventToCall.Invoke(button);
    }
}
