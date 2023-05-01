using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추후에 enemyprojectile과 관련해서 object pool을 적용하자

public class LaunchProjectile : MonoBehaviour
{
    public GameObject _explodeEffect;
    public float _movSpeed;
    public float _lifeTime;
    public float _dmg;

    Rigidbody _rbody;

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rbody.MovePosition(_rbody.position + transform.forward * _movSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            monster.DecreaseHp(_dmg);
            GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
            Destroy(effect, 0.5f);

            Destroy(gameObject, 0.1f);
        }

        else if (other.gameObject.tag == "Wall")
        {
            GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
            Destroy(effect, 0.5f);
            Destroy(gameObject, 0.1f);
        }
    }
}
