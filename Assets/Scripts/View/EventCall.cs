using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventCall : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<EventCall> eventToCall;

    [SerializeField]
    public bool genericSound = false;

    [SerializeField]
    public Button button;

    [SerializeField]
    public Image backgroundImage;

    [SerializeField]
    public TMPro.TextMeshProUGUI textMeshPro;

    [SerializeField]
    public Image image;

    public void OnClick()
    {
        eventToCall.Invoke(this);
        if (genericSound)
            AudioManager.instance.Play("Click2");
    }

    public void Set(string nameToShow, UnityAction action)
    {
        textMeshPro.text = nameToShow;
        button.onClick.AddListener(action);
    }

    public void Set(Sprite spriteToShow, UnityAction action)
    {
        image.sprite = spriteToShow;
        button.onClick.AddListener(action);
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(EventCall eventCall, Button button)
    {
        return eventCall.button == button;
    }

    public static bool operator !=(EventCall eventCall, Button button)
    {
        return eventCall.button != button;
    }

    public static bool operator ==(EventCall eventCall, EventCall button)
    {
        return eventCall.Equals(button);
    }

    public static bool operator !=(EventCall eventCall, EventCall button)
    {
        return !eventCall.Equals(button);
    }


}
