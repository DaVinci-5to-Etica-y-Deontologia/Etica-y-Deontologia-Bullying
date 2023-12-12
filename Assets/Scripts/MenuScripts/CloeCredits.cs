using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloeCredits : MonoBehaviour
{
    [SerializeField]
    GameObject creditsObj;

    [SerializeField]
    GameObject creditsBackButton;
    public void CloseCredits()
    {
        creditsObj.SetActive(false);
    }

    private void OnEnable()
    {
        creditsBackButton.SetActive(true);
    }
}
