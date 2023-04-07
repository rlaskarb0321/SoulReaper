using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NormalMonster : Monster
{
    SphereCollider _traceColl;
    WaitForSeconds _ws;
    NavMeshAgent _navMeshAgent;
    Rigidbody _rbody;
    bool _isTargetSet;

    void Awake()
    {
        _traceColl = GetComponentInChildren<SphereCollider>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rbody = GetComponent<Rigidbody>();

        _ws = new WaitForSeconds(_nextActDelay);
    }

    void Start()
    {
        _traceColl.radius = _monsterBasicStat._traceRadius;
        _monsterState = eMonsterState.Patrol;
        _playerName = _target.gameObject.tag;
    }

    void Update()
    {
        if (!_isTargetSet)
        {
            // Patrol 행동
            StartPatrol();
        }

        if (_isTargetSet && _monsterState != eMonsterState.Attack)
        {
            switch (_monsterState)
            {
                case eMonsterState.Trace:
                    // Trace 행동
                    TraceTarget();
                    break;
                case eMonsterState.Attack:
                    StartCoroutine(DoAttack());
                    break;
            }
        }
    }

    void StartPatrol()
    {

    }

    void TraceTarget()
    {

    }

    public override IEnumerator DoAttack()
    {
        if (_monsterState == eMonsterState.Dead)
            yield break;

        // 공격에관한 행동
        _monsterState = eMonsterState.Attack;

        yield return _ws;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != _playerName)
            return;

        _isTargetSet = true;
        _monsterState = eMonsterState.Trace;
    }
}
