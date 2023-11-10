using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void StartButton()
    {
        TransitionManager.instance.SetTransition("SquaresStart", 0.8f, () => ScenesLoader.instance.LoadDefaultScene());
    }

    public void OptionsButton()
    {
        TransitionManager.instance.SetTransition("Lines", 1.8f);
    }

    public void QuitButton()
    {
        TransitionManager.instance.SetTransition("LinesStart", 1.8f, () => Application.Quit());
    }
}
