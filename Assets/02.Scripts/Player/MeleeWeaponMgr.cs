using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponMgr : MonoBehaviour
{
    public float _atkPower;

    PlayerCombat _combat;
    int _enemyLayer;
    int _enemyProjectile;
    List<GameObject> _hitEnemiesList;

    void Start()
    {
        _combat = GetComponentInParent<PlayerCombat>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        _enemyProjectile = LayerMask.NameToLayer("EnemyProjectile");
        _hitEnemiesList = new List<GameObject>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != _enemyLayer && other.gameObject.layer != _enemyProjectile)
            return;

        // 04.25 몬스터팀들 공격관련
        _hitEnemiesList.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        _hitEnemiesList = _hitEnemiesList.Distinct().ToList();

        for (int i = _hitEnemiesList.Count - 1; i >= 0; i--)
        {
            if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Monster monster = _hitEnemiesList[i].GetComponent<Monster>();
                monster.DecreaseHp(_combat.CalcDamage());
                _hitEnemiesList.Remove(monster.gameObject);
            }
            else if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
            {
                Debug.Log(_hitEnemiesList[i].name + "반사");
                _hitEnemiesList.Remove(_hitEnemiesList[i]);
            }
        }

        _hitEnemiesList.Clear();
    }
}
