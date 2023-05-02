using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LongRangeBehav : Monster
{
    [Header("Long Range Monster")]
    public Transform _firePos;
    public GameObject _projectile;
    [Range(1, 3)] public int _numOfAttacks; // ���Ͱ� ���ϴ� ������ ������
    public float _kitingDist; // �÷��̾ ī�����ϱ����� �־��������ϴ� �Ÿ���

    [Header("Component")]
    AttackEndBehaviour _attackBehaviour;
    ProjectilePool _projectilePool;

    [Header("Field")]
    readonly int _hashAttack1 = Animator.StringToHash("Attack1");
    readonly int _hashIdle = Animator.StringToHash("Idle");
    readonly int _hashMove = Animator.StringToHash("Move");
    bool _isRunAtkCor;

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

        _currActDelay = _basicStat.actDelay;
        _currHp = _basicStat.health;
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
                DefenseSelf();
                break;

            case MonsterAI.eMonsterDesires.Delay:
                // Delay�����϶� �ٺ����� �������ʵ��� ���� �ٶ󸸺��� ��Ŵ
                Vector3 dir = _brain._target.transform.position - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
                break;

            case MonsterAI.eMonsterDesires.Dead:
                return;
        }
    }

    public override IEnumerator DoAttack()
    {
        yield return StartCoroutine(LookTarget());
        yield return new WaitForSeconds(0.15f);

        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashAttack1);

        yield return new WaitUntil(() => _attackBehaviour._isEnd);
        if (_brain.MonsterBrain == MonsterAI.eMonsterDesires.Dead)
            yield break;

        _brain.MonsterBrain = MonsterAI.eMonsterDesires.Delay;
        _isActing = false;
        _attackBehaviour._isEnd = false;
        _isRunAtkCor = false;
    }

    // ���� �ִϸ��̼� ����Ȱ��ȭ���� ��������Ʈ �޼ҵ�
    public override void ExecuteAttack()
    {
        var projectile = _projectilePool._pool.Get();
    }

    // ���� �ִϸ��̼� �������� ��������Ʈ �޼ҵ�
    void SetAttackReady()
    {
        _isActing = false;
    }

    void DefenseSelf()
    {
        StopNav(true);
        if (_brain.DetermineWhethereNeedDefense(Vector3.Distance(_brain._target.position, transform.position), (int)_monsterType))
        {
            _rbody.MovePosition(_rbody.position +
            (transform.position - _brain._target.position).normalized * _basicStat.kitingMovSpeed * Time.deltaTime);
        }
        else
        {
            _brain.MonsterBrain = MonsterAI.eMonsterDesires.Delay;
        }
    }
}