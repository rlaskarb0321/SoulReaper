using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SentryMonster : MonsterType
{
    [Header("=== Sentry ===")]
    public Transform _eyePos;
    [SerializeField] private bool _isSetPatrolPos;
    [SerializeField] private float _missTargetMulti;
    [SerializeField] private float _idleTime;

    [Header("=== MonsterBase ===")]
    public MonsterBase_1 _monsterBase;

    private Vector3 _movPos;
    private float _movSpeed;
    private float _originIdleTime;
    private float _missTargetDist;

    private void Start()
    {
        _missTargetDist = _monsterBase._stat.traceDist * _missTargetMulti;
        _originIdleTime = _idleTime;
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Idle:
                Idle();
                break;

            case MonsterBase_1.eMonsterState.Scout:
                Scout();
                break;

            case MonsterBase_1.eMonsterState.Trace:
                Trace();
                break;
        }
    }

    /// <summary>
    /// 정찰지에 도착한 후, 일정시간 기다림
    /// </summary>
    private void Idle()
    {
        _monsterBase._target = SearchTarget(_monsterBase._stat.traceDist);
        if (_monsterBase._target != null)
        {
            _isSetPatrolPos = false;
            _idleTime = _originIdleTime;
            _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
        }

        if (_idleTime <= 0.0f)
        {
            _isSetPatrolPos = false;
            _idleTime = _originIdleTime;
            _monsterBase._state = MonsterBase_1.eMonsterState.Scout;
            return;
        }

        _monsterBase._animator.SetBool(_monsterBase._hashMove, false);
        _idleTime -= Time.deltaTime;
    }

    /// <summary>
    /// 타겟을 쫓는 메서드, 너무 멀어지면 타겟을 놓친다.
    /// </summary>
    public override void Trace()
    {
        float distance = Vector3.Distance(transform.position, _monsterBase._target.transform.position);

        if (distance >= _missTargetDist || _monsterBase.IsPathPartial(_monsterBase._target.transform.position))
        {
            _monsterBase._target = null;
            _isSetPatrolPos = false;
            _monsterBase._state = MonsterBase_1.eMonsterState.Scout;
            return;
        }

        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
            return;
        }

        _movPos = _monsterBase._target.transform.position;
        _movSpeed = _monsterBase._stat.movSpeed;
        _monsterBase.Move(_movPos, _movSpeed);
    }

    /// <summary>
    /// 정찰을 하도록 하는 메서드
    /// </summary>
    private void Scout()
    {
        _monsterBase._target = SearchTarget(_monsterBase._stat.traceDist);

        if (_monsterBase._target != null)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Trace;
            return;
        }

        if (!_isSetPatrolPos)
        {
            _movPos = SetRandomScout(transform.position, _movPos, _monsterBase._stat.traceDist);
            if (_monsterBase.IsPathPartial(_movPos))
                return;

            _isSetPatrolPos = true;
        }

        // 목적지 도착 확인
        float distance = Vector3.Distance(transform.position, _movPos);
        if (distance <= _monsterBase._nav.stoppingDistance + _monsterBase._nav.baseOffset)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
            return;
        }

        _movSpeed = _monsterBase._stat.movSpeed * 0.5f;
        _monsterBase.Move(_movPos, _movSpeed);
    }

    public override GameObject SearchTarget(float searchRange)
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, searchRange, 1 << LayerMask.NameToLayer("PlayerTeam"));
        return DetectTarget(colls);
    }

    /// <summary>
    /// 자신 주위에 랜덤 포인트를 반환
    /// </summary>
    /// <param name="center">주변을 탐색할 본인의 위치</param>
    /// <param name="destination">목표 좌표</param>
    /// <param name="radius">탐색 범위</param>
    /// <returns>탐색 성공시 목표 위치, 실패시 Vector3.zero</returns>
    private Vector3 SetRandomScout(Vector3 center, Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas))
            {
                destination = hit.position;
                return destination;
            }
        }

        return Vector3.zero;
    }

    /// <summary>
    /// 구형 콜리더 범위안에서 플레이어가 탐지되고, 자신과 비슷한 위치에 있는지 & 물체에 가려지지않았는지 검사
    /// </summary>
    /// <param name="colls">검사할 콜리더 배열</param>
    /// <returns>플레이어가 탐지되면 플레이어를 그렇지않으면 null 을 반환</returns>
    private GameObject DetectTarget(Collider[] colls)
    {
        if (colls.Length < 1)
            return null;

        // 자신과 비슷한 높이에있는지, 타겟이 사물에 가려져있는지 여부 판단 후 타겟 지정
        Vector3 targetVector = colls[0].transform.position - _eyePos.position;
        float distance = targetVector.magnitude;
        Vector3 dir = new Vector3(targetVector.x, 0.0f, targetVector.z);
        RaycastHit hit;
        bool isHit = Physics.Raycast(_eyePos.position, dir, out hit, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("PlayerTeam"));

        if (!isHit)
        {
            // print("nothing");
            return null;
        }

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            return hit.transform.gameObject;
        }

        return null;
    }
}
