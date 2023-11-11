using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventCall : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<Button> eventToCall;

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
        eventToCall.Invoke(button);
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
}
