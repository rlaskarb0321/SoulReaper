using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponMgr : MonoBehaviour
{
    public float _atkPower;
    public GameObject[] _slashEffect;
    public GameObject _fallAttackShockWave;

    List<GameObject> _hitEnemiesList;
    PlayerCombat _combat;
    int _enemyLayer;
    int _enemyProjectile;

    void Start()
    {
        _combat = GetComponentInParent<PlayerCombat>();
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        _enemyProjectile = LayerMask.NameToLayer("EnemyProjectile");
        _hitEnemiesList = new List<GameObject>();
    }

    // 적과 플레이어의 무기가 첫 충돌했을때 이펙트발생시키기 위한 메소드
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != _enemyLayer && other.gameObject.layer != _enemyProjectile)
            return;

        var collisionPoint = other.ClosestPoint(transform.position);
        Instantiate(_slashEffect[0], collisionPoint, Quaternion.identity);
    }

    // 공격에 적중된 적들을 저장해 둘 리스트관련 작업
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != _enemyLayer && other.gameObject.layer != _enemyProjectile)
            return;

        // 몬스터들 공격관련
        _hitEnemiesList.Add(other.gameObject);
    }

    // 저장해 둔 리스트에서 적들의 hp관련 작업
    void OnTriggerExit(Collider other)
    {
        _hitEnemiesList = _hitEnemiesList.Distinct().ToList();

        for (int i = _hitEnemiesList.Count - 1; i >= 0; i--)
        {
            if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                MonsterBase monster = _hitEnemiesList[i].GetComponent<MonsterBase>();
                monster.DecreaseHp(_combat.CalcDamage());
                _hitEnemiesList.Remove(monster.gameObject);
            }
            else if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
            {
                EnemyProjectile projectile = _hitEnemiesList[i].GetComponent<EnemyProjectile>();
                projectile.AllowBaseballHit(_combat.transform.forward);
                _hitEnemiesList.Remove(_hitEnemiesList[i]);
            }
        }

        _hitEnemiesList.Clear();
    }
}
