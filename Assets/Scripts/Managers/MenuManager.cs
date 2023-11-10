using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject optionsMenu;

    public void StartButton()
    {
        TransitionManager.instance.SetTransition(TransitionManager.SquaresStart, 0.8f, () => ScenesLoader.instance.LoadDefaultScene());
    }

    public void OptionsButton()
    {
        TransitionManager.instance.SetTransition(TransitionManager.Lines, 0.8f,() => { optionsMenu.SetActive(true); optionsMenu.GetComponent<CanvasGroup>().alpha = 1; });
    }

    public void QuitButton()
    {
        TransitionManager.instance.SetTransition(TransitionManager.LinesStart, 1.8f, () => Application.Quit());
    }
}
