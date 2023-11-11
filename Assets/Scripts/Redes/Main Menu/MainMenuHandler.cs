using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [Header("Panels")] 
    [SerializeField] private GameObject _joinLobbyPanel;
    [SerializeField] private GameObject _statusPanel;
    [SerializeField] private GameObject _sessionBrowserPanel;
    [SerializeField] private GameObject _hostGamePanel;
    
    [Header("Buttons")]
    [SerializeField] private Button _joinLobbyBTN;
    [SerializeField] private Button _hostPanelBTN;
    [SerializeField] private Button _hostGameBTN;
    
    [Header("InputFields")]
    [SerializeField] private InputField _sessionNameField;

    [SerializeField] private InputField _nicknameField;
    
    [Header("Texts")]
    [SerializeField] private Text _statusText;
    
    void Start()
    {
        _joinLobbyBTN.onClick.AddListener(JoinLobby);
        _hostPanelBTN.onClick.AddListener(ShowHostPanel);
        _hostGameBTN.onClick.AddListener(HostGame);

        _networkRunnerHandler.OnLobbyJoined += () =>
        {
            _statusPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);
        };
    }

    void JoinLobby()
    {
        _networkRunnerHandler.JoinLobby();

        PlayerPrefs.SetString("nickname", _nicknameField.text);
        
        _joinLobbyPanel.SetActive(false);
        _statusPanel.SetActive(true);

        _statusText.text = "Joining Lobby...";
    }

    void ShowHostPanel()
    {
        _sessionBrowserPanel.SetActive(false);
        _hostGamePanel.SetActive(true);
    }

    void HostGame()
    {
        _networkRunnerHandler.CreateGame(_sessionNameField.text, "Game");
    }

}
