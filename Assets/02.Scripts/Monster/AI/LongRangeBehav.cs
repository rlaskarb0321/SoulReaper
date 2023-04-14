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
    ProjectilePool _projectilePool;

    [Header("Field")]
    readonly int _hashAttack1 = Animator.StringToHash("Attack1");

    void Awake()
    {
        _brain = GetComponent<MonsterThink>();
        _animator = GetComponent<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _projectilePool = GetComponent<ProjectilePool>();
    }

    void Update()
    {
        switch (_brain.MonsterBrain)
        {
            case MonsterThink.eMonsterDesires.Patrol:
                break;
            case MonsterThink.eMonsterDesires.Trace:
                TraceTarget();
                break;
            case MonsterThink.eMonsterDesires.Attack:
                if (!_isAttack)
                {
                    _isAttack = true;
                    _nav.isStopped = true;
                    _nav.velocity = Vector3.zero;
                    StartCoroutine(DoAttack());
                }
                break;
            case MonsterThink.eMonsterDesires.Run:
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
        yield return new WaitForSeconds(0.7f);

        _animator.SetTrigger(_hashAttack1);
    }

    public override void ExecuteAttack()
    {
        var projectile = _projectilePool._pool.Get();

        _isAttack = false;
        _nav.isStopped = false;
    }

    //public override IEnumerator DoAttack()
    //{
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
}
