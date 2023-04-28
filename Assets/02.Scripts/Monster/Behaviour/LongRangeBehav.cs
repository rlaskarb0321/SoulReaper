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
                _nav.velocity = Vector3.zero;
                _nav.isStopped = true;
                if (!_isRunAtkCor)
                {
                    _isRunAtkCor = true;
                    StartCoroutine(DoAttack()); 
                }
                break;
            case MonsterAI.eMonsterDesires.Defense:
                StopNav(true);
                // �÷��̾ �ٶ� ��, �������Ӹ�ŭ �ڷ��̵�, �ڷ�ƾ���� brain���°� defense�϶����� �ݺ���Ű��
                if (!_isRunDefCor)
                {
                    _isRunDefCor = true;
                    StartCoroutine(DefenseSelf());
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

        _brain.MonsterBrain = MonsterAI.eMonsterDesires.Trace;
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

    IEnumerator DefenseSelf()
    {
        Vector3 dir = -(_brain._target.position - transform.position).normalized;
        float kitingDist = _kitingDist;

        while (_brain.MonsterBrain == MonsterAI.eMonsterDesires.Defense && kitingDist > 0.0f)
        {
            kitingDist -= Time.deltaTime;
            print(kitingDist);
            _rbody.MovePosition(_rbody.position + dir * _basicStat._kitingMovSpeed * Time.deltaTime);
            yield return null;
        }

        _brain.MonsterBrain = MonsterAI.eMonsterDesires.Trace;
        _isRunDefCor = false;
    }

}
