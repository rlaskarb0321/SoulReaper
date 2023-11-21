using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum eArrowState { Normal, Fire, }

public class LaunchProjectile : MonoBehaviour
{
    public eArrowState _arrowState;
    public GameObject _explodeEffect;
    public GameObject _fireEffect;
    public float _movSpeed;
    public float _lifeTime;
    public float _dmg;
    public BurnDotDamage _burn;

    Rigidbody _rbody;

    void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _arrowState = eArrowState.Normal;
    }

    private void FixedUpdate()
    {
        _rbody.MovePosition(_rbody.position + transform.forward * _movSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Coll"))
            return;
        if (other.gameObject.CompareTag("Fire"))
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            MonsterBase_1 monster = other.GetComponent<MonsterBase_1>();
            monster.DecreaseHP(_dmg, _arrowState == eArrowState.Fire ? _burn : null);
        }

        Boom();
    }

    public void SetArrwInform(float damage) => _dmg = damage; 

    public void UpgradeFire()
    {
        float speed = _movSpeed * 0.5f;

        _arrowState = eArrowState.Fire;
        _fireEffect.SetActive(true);
        _movSpeed = speed;
    }

    public void Boom()
    {
        GameObject effect = Instantiate(_explodeEffect, transform.position, transform.rotation) as GameObject;
        Destroy(effect, 1.5f);
        Destroy(gameObject, 0.05f);
    }
}
