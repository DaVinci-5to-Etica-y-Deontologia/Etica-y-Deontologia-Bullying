using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SummaryItem : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI stateTMP;

    [SerializeField]
    Image logo, background;

    public void SetItem(string text, Sprite logo, Color background)
    {
        stateTMP.text = text;
        this.logo.sprite = logo;
        this.background.color = background;
    }
}
