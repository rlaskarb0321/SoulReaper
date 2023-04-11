using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Combat")]
    [Tooltip("���͵��� ������ ��(= �÷��̾�)")]
    public Transform _target;
    public GameObject _traceCollObj;
    
    [HideInInspector] public NavMeshAgent _navAgent;
    [HideInInspector] public Rigidbody _rbody;
    [HideInInspector] public Monster _monster;
    Animator _animator;
    SphereCollider _traceColl;
    bool _isTargetSet;
    string _playerTag;

    void Awake()
    {
        _traceColl = _traceCollObj.GetComponentInChildren<SphereCollider>();
        _navAgent = GetComponent<NavMeshAgent>();
        _rbody = GetComponent<Rigidbody>();
        _monster = GetComponent<Monster>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _traceColl.radius = _monster._basicStat._traceRadius;
        _monster._state = Monster.eMonsterState.Patrol;
        _playerTag = _target.gameObject.tag;
    }

    void Update()
    {
        if (_monster._state == Monster.eMonsterState.Dead)
            return;

        // ������ ����
        if (_monster._basicStat._isAttackFirst)
        {
            // Ÿ���� �ĺ��ϱ���
            if (!_isTargetSet)
            {
                StartPatrol();
            }

            // Ÿ���� �ĺ��߰� �����ൿ���� �ƴѰ��
            if (_isTargetSet && _monster._state != Monster.eMonsterState.Acting)
            {
                switch (_monster._state)
                {
                    case Monster.eMonsterState.Trace:
                        TraceTarget();
                        break;
                    case Monster.eMonsterState.Attack:
                        StartCoroutine(_monster.DoAttack());
                        break;
                }
            }
        }

        // �񼱰� ����
        else
        {
            // ���ݴ��ϱ���, ���ݴ����Ŀ����� �����ϰ��������� �߰�&������ �� ������ ����
        }
    }

    void StartPatrol()
    {

    }

    void TraceTarget()
    {
        if (_monster._state != Monster.eMonsterState.Trace)
            return;

        if (Vector3.Distance(transform.position, _target.position) > _monster._basicStat._attakableRadius)
            _monster._state = Monster.eMonsterState.Trace;
        else
        {
            _monster._state = Monster.eMonsterState.Attack;
            return;
        }

        if (!_navAgent.pathPending)
        {
            _navAgent.SetDestination(_target.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != _playerTag)
            return;
        if (_isTargetSet)
            return;

        _isTargetSet = true;
        _monster._state = Monster.eMonsterState.Trace;
    }
}
