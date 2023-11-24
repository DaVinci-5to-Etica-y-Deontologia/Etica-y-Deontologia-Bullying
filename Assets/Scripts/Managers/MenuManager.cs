using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [SerializeField] GameObject menuPanel;

    [SerializeField] GameObject optionsMenu;

    [SerializeField] GameObject lobby;

    [SerializeField] private InputField _sessionNameField;

    private void Start()
    {
        _networkRunnerHandler.OnLobbyJoined += () =>
        {
            TransitionManager.instance.SetTransition("SquaresEnd");
            lobby.SetActive(true);
        };
    }


    public void StartButton()
    {
        //TransitionManager.instance.SetTransition(TransitionManager.SquaresStart, 0.8f, () => ScenesLoader.instance.LoadDefaultScene());
        JoinLobby();
    }

    public void OptionsButton()
    {
        menuPanel.SetActive(true);
        TransitionManager.instance.SetTransition(TransitionManager.Lines, 0.8f,() => { optionsMenu.SetActive(true); optionsMenu.GetComponent<CanvasGroup>().alpha = 1; });
    }

    public void QuitButton()
    {
        TransitionManager.instance.SetTransition(TransitionManager.LinesStart, 1.8f, () => Application.Quit());
    }

    void JoinLobby()
    {
        _networkRunnerHandler.JoinLobby();

        //PlayerPrefs.SetString("nickname", _nicknameField.text);

        menuPanel.SetActive(true);
        TransitionManager.instance.SetTransition(TransitionManager.SquaresStart, 0.8f, () => lobby.SetActive(true));
    }

    public void HostGame()
    {
        TransitionManager.instance.SetTransition(TransitionManager.SquaresStart, 0.8f, () => _networkRunnerHandler.CreateGame(Fusion.GameMode.Host, _sessionNameField.text, "SampleScene"));
    }

    public void SingleGame()
    {
        _networkRunnerHandler.JoinLobby();
        TransitionManager.instance.SetTransition(TransitionManager.SquaresStart, 0.8f, () => _networkRunnerHandler.CreateGame(Fusion.GameMode.Single, _sessionNameField.text, "SampleScene"));
    }


}
