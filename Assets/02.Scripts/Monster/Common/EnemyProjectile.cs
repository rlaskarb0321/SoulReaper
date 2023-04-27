using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject _explodeEffect;
    public float _movSpeed;
    public float _lifeTime;
    public int _maxPoolCount;
    [HideInInspector] public bool _isReleased = false;
    public float _dmg;

    IObjectPool<EnemyProjectile> _managedPool;
    Rigidbody _rbody;
    bool _isBaseballHit;
    float _originMovSpeed;

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _originMovSpeed = _movSpeed;
    }

    void OnEnable()
    {
        _movSpeed = _originMovSpeed;
        _isBaseballHit = false;

        StartCoroutine(DestroySelf(_lifeTime));
    }

    void FixedUpdate()
    {
        _rbody.MovePosition(_rbody.position + transform.forward * _movSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isBaseballHit)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Monster monster = other.GetComponent<Monster>();
                monster.DecreaseHp(_dmg);
                GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
                Destroy(effect, 0.5f);

                StartCoroutine(DestroySelf(0.1f));
            }
        }

        if (other.gameObject.tag == "Player")
        {
            GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
            PlayerState player = other.GetComponent<PlayerState>();
            player.GetHit(transform.forward);
            Destroy(effect, 0.5f);
            StartCoroutine(DestroySelf(0.1f));
        }

        else if (other.gameObject.tag == "Wall")
        {
            GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
            Destroy(effect, 0.5f);
            StartCoroutine(DestroySelf(0.1f));
        }
    }

    public void SetManagedPool(IObjectPool<EnemyProjectile> pool)
    {
        _managedPool = pool;
    }

    public void AllowBaseballHit(Vector3 hitDir)
    {
        float hitMovSpeed = _movSpeed * 2.3f;

        _isBaseballHit = true;
        transform.forward = hitDir;
        _movSpeed = hitMovSpeed;
    }

    IEnumerator DestroySelf(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        
        if (!_isReleased)
        {
            _managedPool.Release(this);
            this._isReleased = true;
            gameObject.SetActive(false);
        }
    }
}
