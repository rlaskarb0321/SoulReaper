using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    public AudioClip _hitSound;

    private AudioSource _audio;
    private float _damage;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        _damage = GetComponentInParent<MonsterBase_1>()._stat.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerData player = other.GetComponent<PlayerData>();
            player.DecreaseHP(transform.forward, _damage, _hitSound);
        }
    }
}
