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
    public float _idleTime;

    [Header("Component")]
    AttackEndBehaviour _attackBehaviour;
    ProjectilePool _projectilePool;

    [Header("Field")]
    bool _isRunAtkCor;
    readonly int _hashAttack1 = Animator.StringToHash("Attack1");
    readonly int _hashIdle = Animator.StringToHash("Idle");
    readonly int _hashMove = Animator.StringToHash("Move");

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
        if (_isRunAtkCor)
            yield break;

        _isRunAtkCor = true;
        yield return StartCoroutine(LookTarget());
        yield return new WaitForSeconds(0.1f);

        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashAttack1);

        yield return new WaitUntil(() => _attackBehaviour._isEnd);
        yield return _actWaitSeconds;

        _brain.MonsterBrain = MonsterThink.eMonsterDesires.Trace;
        _isActing = false;
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
}
