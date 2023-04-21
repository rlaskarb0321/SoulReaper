using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터들의 다음 행동 실행을 위해 몬스터 뇌의 욕구를 설정하는 클래스
public class MonsterThink : MonoBehaviour
{
    public enum eMonsterDesires { Patrol, Trace, Attack, Defense, Recover, Retreat, Dead } // 몬스터가 하고자하는 욕구들의 종류
    [SerializeField] private eMonsterDesires _monsterBrain;
    public eMonsterDesires MonsterBrain 
    {
        get { return _monsterBrain; }
        set // MonsterBrain값을 변경하면서 각 상태의맞는 이동속도값으로 수정
        {
            if (value != eMonsterDesires.Attack && _monsterBase._isActing)
                _monsterBase._isActing = false;

            _monsterBrain = value;
            switch (value)
            {
                case eMonsterDesires.Patrol:
                    _monsterBase._movSpeed = _monsterBase._basicStat._patrolMovSpeed;
                    break;
                case eMonsterDesires.Trace:
                    _monsterBase._movSpeed = _monsterBase._basicStat._traceMovSpeed;
                    break;
                case eMonsterDesires.Attack:
                    break;
                case eMonsterDesires.Defense:
                    _monsterBase._movSpeed = _monsterBase._basicStat._kitingMovSpeed;
                    break;
                case eMonsterDesires.Recover:
                    break;
                case eMonsterDesires.Retreat:
                    _monsterBase._movSpeed = _monsterBase._basicStat._retreatMovSpeed;
                    break;
            }
        }
    }
    public bool _isTargetSet;
    [HideInInspector] public Transform _target;

    Monster _monsterBase;
    int _playerTeamLayer;

    void Awake()
    {
        _monsterBase = GetComponent<Monster>();
    }

    void Start()
    {
        _playerTeamLayer = 1 << LayerMask.NameToLayer("PlayerTeam");
        MonsterBrain = eMonsterDesires.Patrol;
    }

    void Update()
    {
        if (MonsterBrain == eMonsterDesires.Dead)
            return;

        DetermineDesires();
    }

    // 몬스터가 다음에 행동하고자 할 행동을 실행하기위해 몬스터의 욕구를 결정
    // 욕구결정에 구현해야하는것 : 몹의 선공여부, 몹의 현재체력값, 타 몬스터의 영혼이 필드에있는지
    void DetermineDesires()
    {
        #region 23/04/17 몬스터 Brain 작동방식 전환
        //Collider[] detectedColls;
        //float targetDist;

        //// 타겟을 감지하지 못했을경우에는 Patrol
        //if (!_isTargetSet)
        //{
        //    // 구체형 콜리더와 닿는것들중에 PlayerTeam이라는 레이어값을 가진 요소들만 배열에 추가
        //    detectedColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius, _playerTeamLayer);

        //    if (detectedColls.Length >= 1)
        //    {
        //        MonsterBrain = eMonsterDesires.Trace;
        //        _isTargetSet = true;
        //        _target = detectedColls[0].transform;
        //    }
        //    else
        //    {
        //        MonsterBrain = eMonsterDesires.Patrol;
        //    }
        //}

        //// 타겟을 감지한경우
        //else
        //{
        //    targetDist = Vector3.Distance(transform.position, _target.position);

        //    // 타겟과의 거리에따라 Attack || Trace
        //    if (targetDist <= _monsterBase._basicStat._attakableRadius)
        //        MonsterBrain = eMonsterDesires.Attack;
        //    else
        //        MonsterBrain = eMonsterDesires.Trace;

        //    // 타겟과의 거리에따라 Attack || Trace
        //    //if (_monsterBase._nav.remainingDistance > _monsterBase._basicStat._attakableRadius)
        //    //    MonsterBrain = eMonsterDesires.Trace;
        //    //else
        //    //    print("공격가능");
        //    //    //MonsterBrain = eMonsterDesires.Attack;
        //}
        #endregion 23/04/17 몬스터 Brain 작동방식 전환

        switch (MonsterBrain)
        {
            case eMonsterDesires.Patrol:
                Collider[] detectColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius,
                    _playerTeamLayer);

                if (detectColls.Length >= 1)
                {
                    MonsterBrain = eMonsterDesires.Trace;
                    _target = detectColls[0].transform;
                }
                break;

            case eMonsterDesires.Attack:
            case eMonsterDesires.Trace:
                if (!_monsterBase._basicStat._isAttackFirst)
                    return;
                if (_monsterBase._isActing)
                    return;

                float targetDist = Vector3.Distance(transform.position, _target.position);

                if (targetDist <= _monsterBase._basicStat._attakableRadius)
                    MonsterBrain = eMonsterDesires.Attack;
                else
                    MonsterBrain = eMonsterDesires.Trace;
                break;

            case eMonsterDesires.Defense:
                break;
            case eMonsterDesires.Recover:
                break;
            case eMonsterDesires.Retreat:
                break;
            default:
                break;
        }
    }
}
