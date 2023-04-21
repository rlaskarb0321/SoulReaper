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
    readonly int _hashAttack1 = Animator.StringToHash("Attack1");

    void Awake()
    {
        _brain = GetComponent<MonsterThink>();
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _projectilePool = GetComponent<ProjectilePool>();
        _attackBehaviour = _animator.GetBehaviour<AttackEndBehaviour>();
    }

    void Start()
    {
        _ws = new WaitForSeconds(_basicStat._actDelay);
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
                case MonsterThink.eMonsterDesires.Patrol:
                    break;
                case MonsterThink.eMonsterDesires.Trace:
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
        if (_isRunAtkCor)
            yield break;

        _isRunAtkCor = true;
        yield return StartCoroutine(LookTarget());
        yield return new WaitForSeconds(0.2f);

        _animator.SetTrigger(_hashAttack1);

        yield return new WaitUntil(() => _attackBehaviour._isEnd);
        yield return _ws;

        _brain.MonsterBrain = MonsterThink.eMonsterDesires.Trace;
        _isActing = false;
        _isRunAtkCor = false;
    }

    public override void ExecuteAttack()
    {
        var projectile = _projectilePool._pool.Get();
    }

    void SetAttackReady()
    {
        _isActing = false;

    }
}
