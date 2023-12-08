using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InroductionGame : MonoBehaviour
{
    [SerializeField] float delayButtons = 0.2f;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Button[] mainButtons = new Button[5];

    [SerializeField] Ease easeType = Ease.OutBounce;
    [SerializeField] float timerToAppear;
    [SerializeField] float timerToMove;

    void Start()
    {
        TimersManager.Create(0.8f, ActiveIntro);

        EnableDisableButtons(false);
    }

    void ActiveIntro()
    {
        title.DOFade(1, 1).OnComplete(ShowButtons); //Title alpha activation
    }

    void ShowButtons()
    {
        mainButtons[0].transform.DOScaleX(1, timerToAppear).SetEase(easeType).SetDelay(1f);
        mainButtons[0].GetComponent<RectTransform>()
        .DOAnchorPos(mainButtons[0].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
        .SetEase(easeType)
        .SetDelay(1.2f)
        .OnComplete(() =>
        {
            mainButtons[1].transform.DOScaleX(1, timerToAppear).SetEase(easeType);
            
            mainButtons[0].GetComponent<RectTransform>()
            .DOAnchorPos(mainButtons[0].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
            .SetDelay(delayButtons)
            .SetEase(easeType);

            mainButtons[1].transform.GetComponent<RectTransform>()
            .DOAnchorPos(mainButtons[1].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
            .SetDelay(delayButtons)
            .SetEase(easeType)

            .OnComplete(() =>
            {
                mainButtons[2].transform.DOScaleX(1, timerToAppear).SetEase(easeType);

                mainButtons[0].GetComponent<RectTransform>()
                .DOAnchorPos(mainButtons[0].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                .SetDelay(delayButtons)
                .SetEase(easeType);

                EnableDisableButtons(true);
                mainButtons[1].transform.GetComponent<RectTransform>()
                .DOAnchorPos(mainButtons[1].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                .SetDelay(delayButtons)
                .SetEase(easeType);

                mainButtons[2].transform.GetComponent<RectTransform>()
                .DOAnchorPos(mainButtons[2].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                .SetDelay(delayButtons)
                .SetEase(easeType)

                .OnComplete(() =>
                {
                    mainButtons[3].transform.DOScaleX(1, timerToAppear).SetEase(easeType);

                    mainButtons[0].GetComponent<RectTransform>()
                    .DOAnchorPos(mainButtons[0].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                    .SetDelay(delayButtons)
                    .SetEase(easeType);

                    EnableDisableButtons(true);
                    mainButtons[1].transform.GetComponent<RectTransform>()
                    .DOAnchorPos(mainButtons[1].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                    .SetDelay(delayButtons)
                    .SetEase(easeType);

                    mainButtons[2].transform.GetComponent<RectTransform>()
                    .DOAnchorPos(mainButtons[2].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                    .SetDelay(delayButtons)
                    .SetEase(easeType);

                    mainButtons[3].transform.GetComponent<RectTransform>()
                    .DOAnchorPos(mainButtons[3].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), timerToMove)
                    .SetDelay(delayButtons)
                    .SetEase(easeType)

                    .OnComplete(() =>
                    {
                        mainButtons[4].transform.DOScaleX(1, timerToAppear).SetEase(easeType);

                        EnableDisableButtons(true);
                    });
                });

            });

        });
    }


    void EnableDisableButtons(bool value)
    {
        foreach (Button button in mainButtons)
        {
            button.interactable = value;
        }
    }

}
