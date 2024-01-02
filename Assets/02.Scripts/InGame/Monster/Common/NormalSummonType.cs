using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISummonType
{
    /// <summary>
    /// 소환할때 관련값들 초기화하는 함수
    /// </summary>
    public void InitUnitData();
    
    public void CompleteSummon();
}

/// <summary>
/// 일반몬스터 소환형
/// </summary>
public class NormalSummonType : MonsterType, ISummonType
{
    public Material _originMat;

    private MonsterBase _monsterBase;

    private void Awake()
    {
        _monsterBase = GetComponent<MonsterBase>();
        _monsterBase._stat.traceDist = 150.0f;
        _monsterBase._target = SearchTarget(_monsterBase._stat.traceDist);
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase.eMonsterState.Dead)
            return;

        switch (_monsterBase._state)
        {
            case MonsterBase.eMonsterState.Trace:
                Trace();
                break;
        }
    }

    public void InitUnitData()
    {
        // 여기에 일반몬스터를 재소환할떄 기본값으로 초기화하는 코드 작성
        Material newMat = Instantiate(_originMat);

        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        _monsterBase._animator.enabled = false;
        _monsterBase._animator.Rebind();
        _monsterBase._animator.enabled = true;
        _monsterBase._nav.enabled = false;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = false;
        _monsterBase.GetComponent<AudioSource>().enabled = false;

        _monsterBase._currHp = _monsterBase._stat.health;
        _monsterBase._state = MonsterBase.eMonsterState.Trace;
        _monsterBase._mesh.material = newMat;
    }

    public override GameObject SearchTarget(float searchRange)
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, searchRange, 1 << LayerMask.NameToLayer("PlayerTeam"));

        return colls[0].gameObject;
    }

    public override void Trace()
    {
        if (!_monsterBase._nav.enabled)
            return;

        float distance = Vector3.Distance(transform.position, _monsterBase._target.transform.position);
        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase.eMonsterState.Attack;
            return;
        }

        _monsterBase.Move(_monsterBase._target.transform.position, _monsterBase._stat.movSpeed);
    }

    public void CompleteSummon()
    {
        _monsterBase._animator.enabled = true;
        _monsterBase._nav.enabled = true;
        _monsterBase.GetComponent<CapsuleCollider>().enabled = true;
        _monsterBase.GetComponent<AudioSource>().enabled = true;
    }
}
