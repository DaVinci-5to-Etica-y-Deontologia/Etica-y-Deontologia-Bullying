using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
/*
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] 
    StreamerManager streamerManager;
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && streamerManager.Count < runner.ActivePlayers.Count())
        {
            streamerManager.CreateStream();
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        runner.Shutdown();
    }

    #region Unused Callbacks
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    
    #endregion
}
*/