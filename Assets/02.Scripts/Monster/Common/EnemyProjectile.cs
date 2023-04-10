using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectile : MonoBehaviour
{
    public GameObject _explodeEffect;
    public float _movSpeed;
    public float _lifeTime;

    public bool _isReleased;
    IObjectPool<EnemyProjectile> _managedPool;
    Rigidbody _rbody;

    void Start()
    {
        _rbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        StartCoroutine(DestroySelf(_lifeTime));
    }

    void FixedUpdate()
    {
        _rbody.MovePosition(_rbody.position + transform.forward * _movSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
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
