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

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        _brain = GetComponent<MonsterThink>();
        _animator = GetComponent<Animator>();
        _attackBoxColl = _attackCollObj.GetComponent<BoxCollider>();
        _attackBehaviour = _animator.GetBehaviour<AttackEndBehaviour>();
    }

    void Start()
    {
        _actWaitSeconds = new WaitForSeconds(_basicStat._actDelay);
    }

    void Update()
    {
        if (_brain.MonsterBrain == MonsterThink.eMonsterDesires.Dead)
            return;

        ActMonster();
    }

    void ActMonster()
    {
        switch (_brain.MonsterBrain)
        {
            case MonsterThink.eMonsterDesires.Idle:
                _animator.SetBool(_hashMove, false);
                _animator.SetTrigger(_hashIdle);
                break;
            case MonsterThink.eMonsterDesires.Patrol:
                if (!_nav.pathPending)
                    _nav.SetDestination(_brain._patrolPos);

                _animator.SetBool(_hashMove, true);
                break;
            case MonsterThink.eMonsterDesires.Trace:
                _animator.SetBool(_hashMove, true);
                TraceTarget();
                break;
            case MonsterThink.eMonsterDesires.Attack:
                if (_isActing)
                    return;

                _isActing = true;
                _nav.isStopped = true;
                _nav.velocity = Vector3.zero;
                StartCoroutine(DoAttack());
                break;
            case MonsterThink.eMonsterDesires.Defense:
                break;
            case MonsterThink.eMonsterDesires.Recover:
                break;
            case MonsterThink.eMonsterDesires.Retreat:
                break;
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

        _brain.MonsterBrain = MonsterThink.eMonsterDesires.Trace;
        _isActing = false;
    }

    // 몬스터의 공격 히트박스관련 Animation Delegate
    public override void ExecuteAttack()
    {
        _attackBoxColl.enabled = !_attackBoxColl.enabled;
    }
}
