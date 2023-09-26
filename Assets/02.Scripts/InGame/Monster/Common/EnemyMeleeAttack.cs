using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    float _damage;

    void Start()
    {
        _damage = GetComponentInParent<MonsterBase_1>()._stat.damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerStat player = other.GetComponent<PlayerStat>();
            player.DecreaseHP(transform.forward, _damage);
        }
    }
}
