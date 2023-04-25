using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponColl : MonoBehaviour
{
    PlayerCombat _combat;
    int _enemyLayer;
    int _enemyProjectile;

    private void Start()
    {
        _combat = GetComponentInParent<PlayerCombat>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        _enemyProjectile = LayerMask.NameToLayer("EnemyProjectile");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != _enemyLayer && other.gameObject.layer != _enemyProjectile)
            return;
        if (_combat._hitEnemiesList.Contains(other.gameObject))
            return;

        // 04.25 몬스터팀들 공격관련
        // _combat._hitEnemiesList.Add(other.gameObject);
    }
}
