using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class SessionBrowserHandler : MonoBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [SerializeField] private Text _statusText;

    [SerializeField] private SessionItem _sessionItemPrefab;

    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

    private void OnEnable()
    {
        _networkRunnerHandler.OnSessionListUpdate += ReceiveSessionList;
    }

    private void OnDisable()
    {
        _networkRunnerHandler.OnSessionListUpdate -= ReceiveSessionList;
    }

    void ReceiveSessionList(List<SessionInfo> allSessions)
    {
        //Limpiar todas las sesiones anteriores (eliminarlas)
        ClearPreviousChildren();

        //Chequear si la lista es nula. En tal caso mostrar el texto y retornar
        if (allSessions.Count == 0)
        {
            NoSessionsFound();
            return;
        }

        //Por cada session, instanciar un prefab con los datos de esa session.
        foreach (var session in allSessions)
        {
            AddNewSessionItem(session);
        }
    }

    void ClearPreviousChildren()
    {
        foreach (GameObject child in _verticalLayoutGroup.transform)
        {
            Destroy(child);
        }

        _statusText.gameObject.SetActive(false);
    }

    void NoSessionsFound()
    {
        _statusText.text = "No sessions found";
        _statusText.gameObject.SetActive(true);
    }

    void AddNewSessionItem(SessionInfo session)
    {
        var newItem = Instantiate(_sessionItemPrefab, _verticalLayoutGroup.transform);
        newItem.SetInfo(session, JoinSelectedSession);
    }
    
    void JoinSelectedSession(SessionInfo session)
    {
        _networkRunnerHandler.JoinGame(session);
    }
}
