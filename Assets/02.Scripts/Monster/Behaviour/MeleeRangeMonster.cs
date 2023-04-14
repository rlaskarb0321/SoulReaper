using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRangeMonster : Monster
{
    [Header("Melee Range Monster")]
    [Range(1, 3)]
    public int _numOfAttacks;

    Animator _animator;
    BoxCollider _attackBoxColl;

    readonly int _hashAttack1 = Animator.StringToHash("Attack1");
    readonly int _hashAttack2 = Animator.StringToHash("Attack2");

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _monsterAI = GetComponent<MonsterAI>();
        _attackBoxColl = GetComponentInChildren<BoxCollider>();
    }

    void Start()
    {
        _playerCombat = _monsterAI._target.GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (_state == Monster.eMonsterState.Dead)
            return;
    }

    public override IEnumerator DoAttack()
    {
        if (_state == Monster.eMonsterState.Dead)
            yield break;

        RigidbodyConstraints originConstraints;
        float randWaitSeconds;

        originConstraints = _monsterAI._rbody.constraints;
        _monsterAI.ManageMonsterNavigation(false, originConstraints);
        _state = eMonsterState.Acting;
        yield return StartCoroutine(LookTarget());

        _animator.SetTrigger(_hashAttack1);
        // randWaitSeconds = _basicStat._attackDelay + Random.Range(-0.8f, 0.2f);
        randWaitSeconds = Random.Range(-0.8f, 0.2f);
        yield return new WaitForSeconds(randWaitSeconds);

        _state = eMonsterState.Trace;
        _monsterAI.ManageMonsterNavigation(true, originConstraints);

        yield return null;
    }

    // 애니메이션 Delegate용 메소드
    public override void ExecuteAttack()
    {
        _attackBoxColl.enabled = !_attackBoxColl.enabled;
    }
}
