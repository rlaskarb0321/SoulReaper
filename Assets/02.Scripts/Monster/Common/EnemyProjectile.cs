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
                MonsterBase monster = other.GetComponent<MonsterBase>();
                // Vector3 hitDir = (other.transform.forward - _rbody.position).normalized;
                monster.DecreaseHp(_dmg);
                GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
                Destroy(effect, 0.5f);

                StartCoroutine(DestroySelf(0.1f));
            }
        }

        if (other.gameObject.tag == "Player")
        {
            GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
            PlayerStat player = other.GetComponent<PlayerStat>();
            player.DecreaseHP(transform.forward, _dmg);
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
        gameObject.SetActive(false);
    }
}
