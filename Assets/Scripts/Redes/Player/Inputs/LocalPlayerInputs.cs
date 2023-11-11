using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerInputs : MonoBehaviour
{
    private NetworkInputData _inputData;

    private bool _isJumpPressed;
    private bool _isFirePressed;
    
    private void Awake()
    {
        _inputData = new NetworkInputData();
    }

    private void Update()
    {
        _inputData.xMovement = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            _isJumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isFirePressed = true;
        }
    }

    public NetworkInputData GetLocalInputs()
    {
        _inputData.isJumpPressed = _isJumpPressed;
        
        _inputData.isFirePressed = _isFirePressed;
        
        _isJumpPressed = _isFirePressed = false;
        
        return _inputData;
    }
}
