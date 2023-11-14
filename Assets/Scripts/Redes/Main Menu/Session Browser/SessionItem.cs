using Fusion;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SessionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sessionName;
    [SerializeField] private TextMeshProUGUI _playersAmount;
    [SerializeField] private Button _joinButton;
    
    public void SetInfo(SessionInfo session, Action<SessionInfo> onClick)
    {
        _sessionName.text = session.Name;

        _playersAmount.text = $"{session.PlayerCount}/{session.MaxPlayers}";
        
        _joinButton.enabled = session.PlayerCount < session.MaxPlayers;

        var myEventCall = GetComponent<EventCall>();

        myEventCall.eventToCall.AddListener((_joinButton) => { onClick(session); TransitionManager.instance.SetTransition("SquaresStart");});
    }
}
