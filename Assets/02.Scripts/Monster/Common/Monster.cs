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
    [Header("Basic Stat")]

    [Tooltip("몬스터의 현재 상태를 나타냄")]
    public eMonsterState _state;
    
    [Tooltip("몬스터의 단계, 계급")]
    public eMonsterLevel _level;
    
    [Tooltip("몬스터들 기본적 스텟 요소")]
    public MonsterBasicStat _basicStat;

    public Transform _projectileSpawnPos;
    public GameObject _projectile;

    public enum eMonsterState { Patrol, Trace, Attack, Acting, Dead, }
    public enum eMonsterLevel { Normal, Elite, MiddleBoss, Boss, }

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

    public virtual IEnumerator LookTarget()
    {
        yield return null;
    }

    public virtual void ExecuteAttack(int attackNum)
    {

    }
}
