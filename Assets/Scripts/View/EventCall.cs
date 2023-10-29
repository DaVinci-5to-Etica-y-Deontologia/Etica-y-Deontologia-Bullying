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

    [SerializeField]
    TMPro.TextMeshProUGUI textMeshPro;

    public void OnClick()
    {
        eventToCall.Invoke(button);
    }

    public void Set(string nameToShow, UnityAction action)
    {
        textMeshPro.text = nameToShow;
        button.onClick.AddListener(action);
    }
}
