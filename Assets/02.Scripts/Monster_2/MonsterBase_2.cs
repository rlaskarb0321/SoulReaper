using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterDefenseState
{
    /// <summary>
    /// 몬스터가 본인을 보호하려할때 실행하는 함수
    /// </summary>
    public virtual void DefenseSelf() { }

    /// <summary>
    /// 방어행동의 쿨타임을 깎는 함수
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator CoolDownDefense() { yield return null; }
}

public abstract class MonsterBase_2 : MonoBehaviour
{
    private Rigidbody _rbody;
    private Animator _animator;

    protected virtual void Awake()
    {
        _rbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {

    }

    #region Move Method
    /// <summary>
    /// 몬스터가 움직이는 방법에대한 함수이다. 
    /// 평범하게 움직이는 몬스터는 구현을 하지않아도되고, 브루들링처럼 특이한 방식으로 움직이는 몬스터는 구현을 해야한다.
    /// </summary>
    public virtual void MovingBehaviour() { }

    /// <summary>
    /// 몬스터가 움직이는 방법을 토대로 실제로 움직이게 해주는 함수
    /// </summary>
    public abstract void MoveMonster();
    #endregion Move Method

    #region Combat Method
    /// <summary>
    /// 플레이어의 공격을 맞았을 떄 호출되는 함수
    /// </summary>
    public abstract void DecreaseHp(float amount);

    /// <summary>
    /// 몬스터의 hp가 0이하일때 실행되는 함수
    /// </summary>
    public abstract void OnMonsterDie();

    /// <summary>
    /// 몬스터가 공격을 하기위해 실행되는 함수
    /// </summary>
    public abstract void DoAttack();

    #endregion Combat Method
}
