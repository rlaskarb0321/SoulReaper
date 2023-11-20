using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
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
            player.DecreaseHP(transform.forward, _damage);
        }
    }
}
