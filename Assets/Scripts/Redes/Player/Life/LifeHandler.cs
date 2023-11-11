using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class LifeHandler : NetworkBehaviour
{
    private const byte FULL_LIFE = 100;
    [Networked(OnChanged = nameof(OnLifeChanged))]
    private byte CurrentLife { get; set; }
    
    [SerializeField] private GameObject _visualObject;
    [SerializeField] private byte _livesAmount = 3;
    
    [Networked(OnChanged = nameof(OnDeadChanged))]
    private bool IsDead { get; set; }

    public event Action OnRespawn = delegate { };
    public event Action<bool> OnEnableController = delegate {  }; 

    public override void Spawned()
    {
        CurrentLife = FULL_LIFE;
    }

    public void TakeDamage(byte dmg)
    {
        if (dmg > CurrentLife) dmg = CurrentLife;
        
        CurrentLife -= dmg;

        if (CurrentLife != 0) return;
        
        _livesAmount--;

        if (_livesAmount == 0)
        {
            DisconnectInputAuthority();
            return;
        }

        StartCoroutine(RespawnCooldown());
    }

    IEnumerator RespawnCooldown()
    {
        IsDead = true;
        
        yield return new WaitForSeconds(2f);

        IsDead = false;

        ApplyRespawn();
    }

    void ApplyRespawn()
    {
        CurrentLife = FULL_LIFE;

        OnRespawn();
    }

    static void OnDeadChanged(Changed<LifeHandler> changed)
    {
        bool currentDead = changed.Behaviour.IsDead;
        
        changed.LoadOld();
        
        bool oldDead = changed.Behaviour.IsDead;

        //Si ahora esta muerto (IsDead == true)
        if (currentDead)
        {
            //Llamo en todos los clientes al metodo de muerte
            changed.Behaviour.RemoteDead();
        }
        else if (oldDead) //Si ahora no estoy muerto pero antes si
        {
            //Llamo en todos los clientes al metodo de respawn
            changed.Behaviour.RemoteRespawn();
        }
    }

    void RemoteDead()
    {
        _visualObject.SetActive(false);

        OnEnableController(false);
    }

    void RemoteRespawn()
    {
        _visualObject.SetActive(true);

        OnEnableController(true);
    }
    
    void DisconnectInputAuthority()
    {
        //Si este objeto que murio no tiene autoridad de input
        if (!Object.HasInputAuthority)
        {
            //Entonces desconecto al cliente que si la tiene sobre este objeto
            Runner.Disconnect(Object.InputAuthority);
        }
        else //Sino, quiero decir que el que murio es el Host
        {
            //Activar el canvas de que perdio el Host
        }

        //Despawneo este jugador ya que murio
        Runner.Despawn(Object);
    }
    
    static void OnLifeChanged(Changed<LifeHandler> changed)
    {
        //TODO: floating life bars.
    }
}
