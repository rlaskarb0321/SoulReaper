using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeMonster : Monster
{
    //[Header("Long Range Monster")]
    //[Range(1, 3)]
    //public int _numOfAttacks;
    //public Transform _projectileSpawnPos;
    //public GameObject _projectile;

    //Animator _animator;
    //ProjectilePool _projectilePool;

    //readonly int _hashAttack1 = Animator.StringToHash("Attack1");
    //readonly int _hashAttack2 = Animator.StringToHash("Attack2");

    //void Awake()
    //{
    //    _projectilePool = GetComponent<ProjectilePool>();
    //    _animator = GetComponent<Animator>();
    //    _monsterAI = GetComponent<MonsterAI>();
    //}

    //void Update()
    //{
    //    if (_state == Monster.eMonsterState.Dead)
    //        return;
    //}

    //public override IEnumerator DoAttack()
    //{
    //    if (_state == eMonsterState.Dead)
    //        yield break;

    //    RigidbodyConstraints originConstraints;
    //    float randWaitSeconds;

    //    originConstraints = _monsterAI._rbody.constraints;
    //    _monsterAI.ManageMonsterNavigation(false, originConstraints);
    //    _state = eMonsterState.Acting;
    //    yield return StartCoroutine(LookTarget());

    //    _animator.SetTrigger(_hashAttack1);
    //    // randWaitSeconds = _basicStat._attackDelay + Random.Range(-0.8f, 0.8f);
    //    randWaitSeconds = Random.Range(-0.8f, 0.8f);
    //    yield return new WaitForSeconds(randWaitSeconds);

    //    _state = eMonsterState.Trace;
    //    _monsterAI.ManageMonsterNavigation(true, originConstraints);
    //}

    // 애니메이션 Delegate용 메소드
}
