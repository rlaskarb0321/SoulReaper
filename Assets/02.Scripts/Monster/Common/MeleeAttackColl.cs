using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackColl : MonoBehaviour
{
    PlayerCombat _playerCombat;

    void Start()
    {
        _playerCombat = GetComponentInParent<MonsterAI>()._target.GetComponent<PlayerCombat>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("¸ÂÀ½");
            StartCoroutine(_playerCombat.GetHit(transform.forward));
        }
    }
}
