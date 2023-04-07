using System.Collections;
using UnityEngine;

[System.Serializable]
public struct MonsterBasicStat
{
    public int _health; // 체력
    public bool _isAttackFirst; // 선공or비선공여부
    public float _attackDelay; // 공격후 다음공격까지 기다려야하는 시간값
    public float _traceRadius; // 추격을 인지하는 범위
    public float _attakableRadius; // 공격사정거리
}

/// <summary>
/// 모든 몬스터들이 가져야하는 기본요소
/// </summary>
public class Monster : MonoBehaviour
{
    public enum eMonsterState { Patrol, Trace, Attack, Dead, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

    [Tooltip("몬스터의 현재 상태를 나타냄")]
    public eMonsterState _monsterState;
    
    [Tooltip("몬스터의 단계, 계급")]
    public eMonsterLevel _level;
    
    [Tooltip("몬스터들 기본적 스텟 요소")]
    public MonsterBasicStat _monsterBasicStat;

    [Tooltip("몬스터들 다음 행동까지 걸리게 할 시간")]
    public float _nextActDelay;

    [Header("Target")]
    [Tooltip("몬스터들의 추적할 적(= 플레이어)")]
    public Transform _target;

    protected string _playerName;

    public virtual void DecreaseHp(float amount)
    {

    }

    public virtual void Dead()
    {

    }

    public virtual IEnumerator DoAttack()
    {
        yield return null;
    }
}
