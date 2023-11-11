using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerGun : NetworkBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _spawningBullet;
    [SerializeField] private ParticleSystem _fireParticles;

    [SerializeField] private float _shootCooldown = 0.15f;

    private float _lastShootTime;

    [Networked (OnChanged = nameof(OnFiringChanged))]
    private bool IsFiring { get; set; }
    
    public void Shoot()
    {
        if (Time.time - _lastShootTime < _shootCooldown) return;

        _lastShootTime = Time.time;
        
        StartCoroutine(ShootCooldown());
        Runner.Spawn(_bulletPrefab, _spawningBullet.position, transform.rotation);

        #region CON RAYCAST
        // var raycast = Runner.LagCompensation.Raycast(origin: _spawningBullet.position,
        //                                                 direction: _spawningBullet.forward,
        //                                                 length: 100,
        //                                                 player: Object.InputAuthority,
        //                                                 hit: out var hitInfo);
        //
        // if (!raycast) return;
        //
        // Debug.Log(hitInfo.Hitbox.Root.gameObject.name);
        // hitInfo.GameObject.GetComponentInParent<LifeHandler>()?.TakeDamage(25);
        #endregion
    }

    IEnumerator ShootCooldown()
    {
        IsFiring = true;
        yield return new WaitForSeconds(_shootCooldown);
        IsFiring = false;
    }
    
    static void OnFiringChanged(Changed<PlayerGun> changed)
    {
        bool currentFiring = changed.Behaviour.IsFiring;
        changed.LoadOld();
        bool oldFiring = changed.Behaviour.IsFiring;

        if (!oldFiring && currentFiring) changed.Behaviour.TurnOnShootingParticle();
    }

    void TurnOnShootingParticle()
    {
        _fireParticles.Play();
    }
}
