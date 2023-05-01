using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeBehav : Monster
{
    [Header("Long Range Monster")]
    public Transform _firePos;
    public GameObject _projectile;
    [Range(1, 3)] public int _numOfAttacks; // 몬스터가 행하는 공격의 가짓수
    public float _kitingDist; // 플레이어를 카이팅하기위해 멀어지고자하는 거리값

    [Header("Component")]
    AttackEndBehaviour _attackBehaviour;
    ProjectilePool _projectilePool;

    [Header("Field")]
    bool _isRunAtkCor;
    bool _isRunDefCor;
    readonly int _hashAttack1 = Animator.StringToHash("Attack1");
    readonly int _hashIdle = Animator.StringToHash("Idle");
    readonly int _hashMove = Animator.StringToHash("Move");

    protected override void Awake()
    {
        base.Awake();

        _brain = GetComponent<MonsterAI>();
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _projectilePool = GetComponent<ProjectilePool>();
        _attackBehaviour = _animator.GetBehaviour<AttackEndBehaviour>();
    }

    protected override void Start()
    {
        base.Start();

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
                _nav.velocity = Vector3.zero;
                _nav.isStopped = true;
                if (!_isRunAtkCor)
                {
                    _isRunAtkCor = true;
                    StartCoroutine(DoAttack());
                }
                break;
            case MonsterAI.eMonsterDesires.Defense:
                if (!_isRunDefCor)
                {
                    _isRunDefCor = true;
                    StopNav(true);
                    StartCoroutine(KiteFromPlayer());
                }
                break;
            case MonsterAI.eMonsterDesires.Dead:
                return;
        }
    }

    public override IEnumerator DoAttack()
    {
        yield return StartCoroutine(LookTarget());
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashAttack1);

        yield return new WaitUntil(() => _attackBehaviour._isEnd);
        yield return _actWaitSeconds;

        if (_brain.MonsterBrain == MonsterAI.eMonsterDesires.Dead)
            yield break;

        if (_brain.DetermineWhethereNeedDefense(Vector3.Distance(transform.position, _brain._target.position), (int)_monsterType))
            _brain.MonsterBrain = MonsterAI.eMonsterDesires.Defense;
        else
            _brain.MonsterBrain = MonsterAI.eMonsterDesires.Trace;

        _isActing = false;
        _isRunAtkCor = false;
    }

    // 공격 애니메이션 공격활성화시점 델리게이트 메소드
    public override void ExecuteAttack()
    {
        var projectile = _projectilePool._pool.Get();
    }

    // 공격 애니메이션 마무리용 델리게이트 메소드
    void SetAttackReady()
    {
        _isActing = false;
    }

    IEnumerator KiteFromPlayer()
    {
        while (true)
        {
            if (_brain.MonsterBrain != MonsterAI.eMonsterDesires.Defense)
            {
                yield return _actWaitSeconds;
                _isRunDefCor = false;
                yield break;
            }

            Vector3 runDir = (transform.position - _brain._target.position).normalized;
            _rbody.MovePosition(_rbody.position + runDir * _nav.speed * 2.7f * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime * 3.0f);
        }
    }
}