using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponMgr : MonoBehaviour
{
    [Space(10.0f)]
    public float _atkPower;
    public GameObject[] _slashEffect;
    public GameObject _shockWave;
    [HideInInspector]
    public SoundEffects _sfx;

    private List<GameObject> _hitEnemiesList;
    private PlayerCombat _combat;
    private PlayerData _playerData;
    private int _enemyLayer;
    private int _enemyProjectile;

    private void Awake()
    {
        _combat = GetComponentInParent<PlayerCombat>();
        _playerData = GetComponentInParent<PlayerData>();
        _sfx = GetComponent<SoundEffects>();
    }

    private void Start()
    {
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
        GameObject slashEffect = Instantiate(_slashEffect[0], collisionPoint, Quaternion.identity);

        Destroy(slashEffect, 1.0f);
        _sfx.PlayOneShotUsingDict("Attack Hit");
    }

    // 공격에 적중된 적들을 저장해 둘 리스트관련 작업
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != _enemyLayer && other.gameObject.layer != _enemyProjectile)
            return;

        // 몬스터들 공격관련
        _hitEnemiesList.Add(other.gameObject);
    }

    #region 23.09.11 다시 부활한 두번 타격, 큰 몬스터 때릴때 발생
    // 저장해 둔 리스트에서 적들의 hp관련 작업
    //void OnTriggerExit(Collider other)
    //{
    //    _hitEnemiesList = _hitEnemiesList.Distinct().ToList();

    //    for (int i = _hitEnemiesList.Count - 1; i >= 0; i--)
    //    {
    //        if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
    //        {
    //            MonsterBase monster = _hitEnemiesList[i].GetComponent<MonsterBase>();
    //            monster.DecreaseHp(_combat.CalcDamage(), _combat.transform.position);
    //            //_hitEnemiesList.Remove(monster.gameObject);
    //        }
    //        else if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
    //        {
    //            EnemyProjectile projectile = _hitEnemiesList[i].GetComponent<EnemyProjectile>();
    //            projectile.AllowBaseballHit(_combat.transform.forward);
    //            //_hitEnemiesList.Remove(_hitEnemiesList[i]);
    //        }
    //    }

    //    _hitEnemiesList.Clear();
    //}
    #endregion 23.09.11 다시 부활한 두번 타격, 큰 몬스터 때릴때 발생

    public void DecreaseHitMonster()
    {
        if (_hitEnemiesList.Count == 0)
            return;

        _hitEnemiesList = _hitEnemiesList.Distinct().ToList();

        for (int i = _hitEnemiesList.Count - 1; i >= 0; i--)
        {
            if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                MonsterBase_1 monster = _hitEnemiesList[i].GetComponent<MonsterBase_1>();
                if (monster._currHp == 0)
                    return;

                monster.DecreaseHP(_combat.CalcDamage());
                StartCoroutine(monster.OnHitEvent());

                _playerData.DecreaseMP(-10.0f);
                _hitEnemiesList.Remove(monster.gameObject);
            }
            else if (_hitEnemiesList[i].gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
            {
                EnemyProjectile projectile = _hitEnemiesList[i].GetComponent<EnemyProjectile>();
                projectile.ReflectProjectile(_combat.transform.forward);
                _hitEnemiesList.Remove(_hitEnemiesList[i]);
            }
        }

        _hitEnemiesList.Clear();
    }

    public void ResetHitEnemy()
    {
        _hitEnemiesList.Clear();
    }
}
