using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerGun))]
[RequireComponent(typeof(LifeHandler))]
public class PlayerController : NetworkBehaviour
{
    private PlayerMovement _playerMovement;
    private PlayerGun _playerGun;
    private NetworkInputData _networkInput;

    private Vector3 _direction;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerGun = GetComponent<PlayerGun>();
        
        GetComponent<LifeHandler>().OnEnableController += (b) => enabled = b;
    }
    
    private void OnEnable()
    {
        if (!_playerMovement.Controller) return;
        
        _playerMovement.Controller.enabled = true;
    }

    private void OnDisable()
    {
        if (!_playerMovement.Controller) return;
        
        _playerMovement.Controller.enabled = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out _networkInput)) return;

        //MOVEMENT
        _direction = Vector3.forward * _networkInput.xMovement;
        _playerMovement.Move(_direction);
        
        //JUMP
        if (_networkInput.isJumpPressed)
        {
            _playerMovement.Jump();
        }
        
        //SHOOT
        if (_networkInput.isFirePressed)
        {
            _playerGun.Shoot();
        }
    }
}
