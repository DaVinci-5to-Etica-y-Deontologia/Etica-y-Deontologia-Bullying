using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Bullet : NetworkRigidbody
{
    private TickTimer _expireLifeTimer = TickTimer.None;
    
    public override void Spawned()
    {
        base.Spawned();
        
        Rigidbody.AddForce(transform.forward * 10f, ForceMode.VelocityChange);

        if (Object.HasStateAuthority)
            _expireLifeTimer = TickTimer.CreateFromSeconds(Runner, 2f);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (!Object.HasStateAuthority) return;

        if (_expireLifeTimer.Expired(Runner))
        {
            DespawnObject();
        }
    }

    void DespawnObject()
    {
        _expireLifeTimer = TickTimer.None;
        Runner.Despawn(Object);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!Object || !Object.HasStateAuthority) return;
        
        if (other.TryGetComponent(out LifeHandler enemy))
        {
            enemy.TakeDamage(25);
        }

        DespawnObject();
    }
}
