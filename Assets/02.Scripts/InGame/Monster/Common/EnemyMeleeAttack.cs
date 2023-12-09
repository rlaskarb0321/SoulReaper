using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public AudioClip _hitSound;
    public GameObject _hitEffect;

    private float _damage;

    void Start()
    {
        _damage = GetComponentInParent<MonsterBase_1>()._stat.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerData player = other.GetComponent<PlayerData>();
            GameObject hitPrefab = Instantiate(_hitEffect, transform.position, Quaternion.identity);

            Destroy(hitPrefab, 1.0f);
            player.DecreaseHP(transform.forward, _damage, _hitSound);
        }
    }
}
