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
        _damage = GetComponentInParent<MonsterBase>()._stat.damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerData player = other.GetComponent<PlayerData>();
            Vector3 randomPos = Random.insideUnitCircle;
            randomPos = new Vector3(randomPos.x, Mathf.Abs(randomPos.y), randomPos.z);
            GameObject hitPrefab = Instantiate(_hitEffect, other.transform.position + randomPos, Quaternion.identity);

            Destroy(hitPrefab, 1.0f);
            player.DecreaseHP(transform.forward, _damage, _hitSound);
        }
    }
}
