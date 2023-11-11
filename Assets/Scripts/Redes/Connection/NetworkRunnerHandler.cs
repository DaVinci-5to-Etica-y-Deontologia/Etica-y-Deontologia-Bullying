using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner _runnerPrefab;
    private NetworkRunner _currentRunner;

    public event Action OnLobbyJoined = delegate { };
    public event Action<List<SessionInfo>> OnSessionListUpdate = delegate { };

    #region LOBBY

    public void JoinLobby()
    {
        if (_currentRunner) Destroy(_currentRunner.gameObject);

        _currentRunner = Instantiate(_runnerPrefab);
        
        _currentRunner.AddCallbacks(this);

        var clientTask = JoinLobbyTask();
    }

    async Task JoinLobbyTask()
    {
        var result = await _currentRunner.JoinSessionLobby(SessionLobby.Custom, "Normal Lobby");

        if (result.Ok)
        {
            OnLobbyJoined();
        }
        else
        {
            Debug.LogError("[Custom Error] Unable to join Lobby");
        }
    }

    #endregion

    #region CREATE/JOIN GAME
    
    public void CreateGame(string sessionName, string sceneName)
    {
        var clientTask = InitializeGame(GameMode.Host, sessionName, SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}"));
    }

    public void JoinGame(SessionInfo session)
    {
        var clientTask = InitializeGame(GameMode.Client, session.Name);
    }
    
    async Task InitializeGame(GameMode gameMode, string sessionName, SceneRef? sceneToLoad = null)
    {
        var sceneManager = _currentRunner.GetComponent<NetworkSceneManagerDefault>();

        _currentRunner.ProvideInput = true;

        var result = await _currentRunner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName= sessionName,
            Scene = sceneToLoad,
            CustomLobbyName = "Normal Lobby",
            SceneManager = sceneManager
        });

        if (result.Ok)
        {
            Debug.Log("[Custom Msg] Game Created/Joined");
        }
        else
        {
            Debug.LogError("[Custom Error] Unable to create/join Game");
        }
    }
    
    #endregion
    
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        OnSessionListUpdate(sessionList);
        
        // if (sessionList.Count > 0)
        // {
        //     var sessionInfo = sessionList[0];
        //     
        //     JoinGame(sessionInfo);
        // }
    }

    #region Unused Callbacks
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    
    #endregion
}
