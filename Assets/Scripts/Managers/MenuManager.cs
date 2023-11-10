using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject optionsMenu;

    public void StartButton()
    {
        TransitionManager.instance.SetTransition("SquaresStart", 0.8f, () => ScenesLoader.instance.LoadDefaultScene());
    }

    public void OptionsButton()
    {
        TransitionManager.instance.SetTransition("Lines", 0.8f,() => { optionsMenu.SetActive(true); optionsMenu.GetComponent<CanvasGroup>().alpha = 1; });
    }

    public void QuitButton()
    {
        TransitionManager.instance.SetTransition("LinesStart", 1.8f, () => Application.Quit());
    }
}
