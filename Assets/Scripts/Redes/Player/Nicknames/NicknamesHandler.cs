using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicknamesHandler : MonoBehaviour
{
    public static NicknamesHandler Instance { get; private set; }
    
    [SerializeField] private NicknameItem _nicknamePrefab;

    private List<NicknameItem> _allNicknames;

    private void Awake()
    {
        Instance = this;
        
        _allNicknames = new List<NicknameItem>();
    }

    public NicknameItem CreateNewNickname(NetworkPlayer owner)
    {
        var newNickname = Instantiate(_nicknamePrefab, transform);
        _allNicknames.Add(newNickname);
        
        newNickname.SetOwner(owner.transform);

        owner.OnPlayerDespawned += () =>
        {
            _allNicknames.Remove(newNickname);

            Destroy(newNickname.gameObject);
        };
        
        return newNickname;
    }
    
    private void LateUpdate()
    {
        foreach (var nick in _allNicknames)
        {
            nick.UpdatePosition();
        }
    }
}
