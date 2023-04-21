//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class MonsterAI : MonoBehaviour
//{
//    [Header("Combat")]
//    [Tooltip("몬스터들의 추적할 적(= 플레이어)")]
//    public Transform _target;
//    public GameObject _traceCollObj;
//    public float _idleTime;
    
//    [HideInInspector] public NavMeshAgent _navAgent;
//    [HideInInspector] public Rigidbody _rbody;
//    [HideInInspector] public Monster _monster;
//    Animator _animator;
//    SphereCollider _traceColl;
//    Vector3 _patrollDestination;
//    bool _isTargetSet;
//    string _playerTag;

//    void Awake()
//    {
//        _traceColl = _traceCollObj.GetComponentInChildren<SphereCollider>();
//        _navAgent = GetComponent<NavMeshAgent>();
//        _rbody = GetComponent<Rigidbody>();
//        _monster = GetComponent<Monster>();
//        _animator = GetComponent<Animator>();

//        _target = GameObject.Find("PlayerCharacter").transform;
//    }

//    void Start()
//    {
//        _traceColl.radius = _monster._basicStat._traceRadius;
//        _monster._state = Monster.eMonsterState.Patrol;
//        _playerTag = _target.gameObject.tag;
//    }

//    void Update()
//    {
//        if (_monster._state == Monster.eMonsterState.Dead)
//            return;

//        // 선공형 몬스터
//        if (_monster._basicStat._isAttackFirst)
//        {
//            if (!_isTargetSet)
//            {

//            }

//            // 타겟을 식별했고 공격행동중이 아닌경우
//            if (_isTargetSet && _monster._state != Monster.eMonsterState.Acting)
//            {
//                switch (_monster._state)
//                {
//                    case Monster.eMonsterState.Trace:
//                        TraceTarget();
//                        break;
//                    case Monster.eMonsterState.Attack:
//                        StartCoroutine(_monster.DoAttack());
//                        break;
//                }
//            }
//        }

//        // 비선공 몬스터
//        else
//        {
//            // 공격당하기전, 공격당한후에따라 정찰하고있을건지 추격&공격을 할 것인지 결정
//        }
//    }

//    // 타겟이 식별되기 전이고, 정찰지가 정해진게 아니면 정찰지를 정해준다
//    // 정찰지가 정해지면 해당지역으로 이동을 마치기전까지 정찰지를 정하지않는다.
//    // 정찰지까지 이동을 마치면 잠시 유휴시간을 갖는다.
//    void StartPatrolling()
//    {
//        if (_monster._state == Monster.eMonsterState.Patrol)
//        {
//            if (_navAgent.remainingDistance > _navAgent.stoppingDistance)
//            {
//                _navAgent.SetDestination(_patrollDestination);
//                return;
//            }
//            else
//            {
//                // 잠시 유휴시간을 가져야함
//                _monster._state = Monster.eMonsterState.Idle;
//                return;
//            }
//        }

//        NavMeshHit navHit;
//        Vector3 randomDir = Random.insideUnitSphere * 10.0f;

//        randomDir += transform.position;
//        NavMesh.SamplePosition(randomDir, out navHit, 10.0f, NavMesh.AllAreas);
//        _patrollDestination = navHit.position;

//        if (!_navAgent.pathPending)
//        {
//            _navAgent.SetDestination(_patrollDestination);
//            _monster._state = Monster.eMonsterState.Patrol;
//        }
//    }

//    IEnumerator IdleMonster()
//    {
//        if (_monster._state != Monster.eMonsterState.Idle)
//            yield break;

//        float randomValue = Random.Range(-0.5f, 1.1f);
//        yield return new WaitForSeconds(_idleTime + randomValue);
//    }

//    void StartPatrol()
//    {
//        NavMeshHit navHit;
//        Vector3 destination;

//        NavMesh.SamplePosition(transform.position, out navHit, 50.0f, NavMesh.AllAreas);
//        destination = navHit.position;
//        Debug.Log(destination);

//        if (!_navAgent.pathPending)
//        {
//            _navAgent.SetDestination(destination); 
//        }
//    }

//    void TraceTarget()
//    {
//        if (_monster._state != Monster.eMonsterState.Trace)
//            return;

//        if (Vector3.Distance(transform.position, _target.position) > _monster._basicStat._attakableRadius)
//            _monster._state = Monster.eMonsterState.Trace;
//        else
//        {
//            _monster._state = Monster.eMonsterState.Attack;
//            return;
//        }

//        if (!_navAgent.pathPending)
//        {
//            _navAgent.SetDestination(_target.position);
//        }
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.tag != _playerTag)
//            return;
//        if (_isTargetSet)
//            return;

//        _isTargetSet = true;
//        _monster._state = Monster.eMonsterState.Trace;
//    }

//    public void ManageMonsterNavigation(bool isOn, RigidbodyConstraints originConstValue)
//    {
//        _navAgent.enabled = isOn;

//        if (isOn == false)
//        {
//            _rbody.constraints = RigidbodyConstraints.FreezePositionY | originConstValue;
//        }
//        else
//        {
//            _rbody.constraints = originConstValue;
//        }
//    }
//}
