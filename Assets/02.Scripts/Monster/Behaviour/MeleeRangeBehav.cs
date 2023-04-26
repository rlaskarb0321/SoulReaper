using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeRangeBehav : Monster
{
    [Header("Melee Range Monster")]
    [Range(1, 3)]
    public int _numOfAttacks;
    public GameObject _attackCollObj;

    AttackEndBehaviour _attackBehaviour;
    BoxCollider _attackBoxColl;
    readonly int _hashAttack1 = Animator.StringToHash("Attack1");
    readonly int _hashIdle = Animator.StringToHash("Idle");
    readonly int _hashMove = Animator.StringToHash("Move");

    protected override void Awake()
    {
        base.Awake();

        _nav = GetComponent<NavMeshAgent>();
        _brain = GetComponent<MonsterAI>();
        _animator = GetComponent<Animator>();
        _attackBoxColl = _attackCollObj.GetComponent<BoxCollider>();
        _attackBehaviour = _animator.GetBehaviour<AttackEndBehaviour>();
    }

    void Start()
    {
        _actWaitSeconds = new WaitForSeconds(_basicStat._actDelay);
        _currHp = _basicStat._health;
    }

    void Update()
    {
        if (_brain.MonsterBrain == MonsterAI.eMonsterDesires.Dead)
            return;

        ActMonster();
    }

    void ActMonster()
    {
        switch (_brain.MonsterBrain)
        {
            case MonsterAI.eMonsterDesires.Idle:
                _animator.SetBool(_hashMove, false);
                _animator.SetTrigger(_hashIdle);
                break;
            case MonsterAI.eMonsterDesires.Patrol:
                if (!_nav.pathPending)
                    _nav.SetDestination(_brain._patrolPos);

                _animator.SetBool(_hashMove, true);
                break;
            case MonsterAI.eMonsterDesires.Trace:
                _animator.SetBool(_hashMove, true);
                TraceTarget();
                break;
            case MonsterAI.eMonsterDesires.Attack:
                if (_isActing)
                    return;

                _isActing = true;
                _nav.isStopped = true;
                _nav.velocity = Vector3.zero;
                StartCoroutine(DoAttack());
                break;
            case MonsterAI.eMonsterDesires.Defense:
                break;
            case MonsterAI.eMonsterDesires.Recover:
                break;
            case MonsterAI.eMonsterDesires.Retreat:
                break;
            case MonsterAI.eMonsterDesires.Dead:
                StopCoroutine(DoAttack());
                return;
        }
    }

    public override IEnumerator DoAttack()
    {
        yield return StartCoroutine(LookTarget());
        yield return new WaitForSeconds(0.05f);

        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashAttack1);

        yield return new WaitUntil(() => _attackBehaviour._isEnd);
        yield return _actWaitSeconds;

        if (_brain.MonsterBrain == MonsterAI.eMonsterDesires.Dead)
            yield break;

        _brain.MonsterBrain = MonsterAI.eMonsterDesires.Trace;
        _isActing = false;
    }

    // 몬스터의 공격 히트박스관련 Animation Delegate
    public override void ExecuteAttack()
    {
        _attackBoxColl.enabled = !_attackBoxColl.enabled;
    }
}
