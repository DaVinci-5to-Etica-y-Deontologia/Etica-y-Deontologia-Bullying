using UnityEngine;
using Fusion;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }

    public event Action OnPlayerDespawned = delegate { };

    private NicknameItem _myNickname;
    
    [Networked(OnChanged = nameof(OnNicknameChanged))]
    private string Nickname { get; set; } 
    
    public override void Spawned()
    {
        _myNickname = NicknamesHandler.Instance.CreateNewNickname(this);
        
        if (Object.HasInputAuthority)
        {
            Local = this;

            var savedNickname = PlayerPrefs.GetString("nickname");

            RPC_SetNewNickname(savedNickname);
            
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;
        }
        else
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_SetNewNickname(string newNick)
    {
        Nickname = newNick;
    }
    
    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        var behaviour = changed.Behaviour;
        
        behaviour._myNickname.UpdateNickname(behaviour.Nickname);
    }
    
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnPlayerDespawned();
    }
}
