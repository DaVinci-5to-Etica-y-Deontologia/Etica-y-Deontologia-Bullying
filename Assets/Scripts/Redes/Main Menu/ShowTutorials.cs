using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ShowTutorials : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    TextMeshProUGUI tutorialText;

    [SerializeField]
    Image imageToShow;

    [SerializeField]
    BD dataBase;

    Tutos[] tutos => dataBase.Tutos;

    int index = -1;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeTuto();
        AudioManager.instance.Play("Click4");
    }

    void ChangeTuto()
    {
        index++;

        if (index >= tutos.Length)
            index = 0;

        imageToShow.sprite = tutos[index].sprite;

        tutorialText.text = tutos[index].texts.ToString();
    }

    private void OnEnable()
    {
        index = -1;
        ChangeTuto();
    }
}
