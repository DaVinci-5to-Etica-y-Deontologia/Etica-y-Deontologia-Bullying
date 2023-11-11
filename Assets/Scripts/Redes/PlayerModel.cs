using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Rigidbody _rgbd;
    [SerializeField] private Animator _animator;
    
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private ParticleSystem _shootParticle;
    [SerializeField] private Transform _shootPosition;

    [SerializeField] private float _life;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;

    private float _xAxi;

    private int _currentSign, _previousSign;
    
    void Start()
    {
        transform.forward = Vector3.right;
    }

    void Update()
    {
        _xAxi = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (_xAxi != 0)
        {
            _rgbd.MovePosition(transform.position + Vector3.right * (_xAxi * _speed * Time.fixedDeltaTime));

            _currentSign = (int)Mathf.Sign(_xAxi);

            if (_currentSign != _previousSign)
            {
                _previousSign = _currentSign;

                transform.rotation = Quaternion.Euler(Vector3.up * (90 * _currentSign));
            }
            
            _animator.SetFloat("MovementValue", Mathf.Abs(_xAxi));
        }
        else if (_currentSign != 0)
        {
            _currentSign = 0;
            _animator.SetFloat("MovementValue", 0);
        }
    }
    
    void Jump()
    {
        _rgbd.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }
    
    void Shoot()
    {
        Instantiate(_bulletPrefab, _shootPosition.position, transform.rotation);
        _shootParticle.Play();
    }

    public void TakeDamage(float dmg)
    {
        
    }
}
