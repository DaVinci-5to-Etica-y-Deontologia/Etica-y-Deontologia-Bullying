using Fusion;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SessionItem : MonoBehaviour
{
    [SerializeField] private Text _sessionName;
    [SerializeField] private Text _playersAmount;
    [SerializeField] private Button _joinButton;
    
    public void SetInfo(SessionInfo session, Action<SessionInfo> onClick)
    {
        _sessionName.text = session.Name;

        _playersAmount.text = $"{session.PlayerCount}/{session.MaxPlayers}";
        
        _joinButton.enabled = session.PlayerCount < session.MaxPlayers;
        
        _joinButton.onClick.AddListener(() => onClick(session));
    }
}
