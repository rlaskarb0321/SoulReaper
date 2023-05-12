using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterDefenseState
{
    /// <summary>
    /// ���Ͱ� ������ ��ȣ�Ϸ��Ҷ� �����ϴ� �Լ�
    /// </summary>
    public virtual void DefenseSelf() { }

    /// <summary>
    /// ����ൿ�� ��Ÿ���� ��� �Լ�
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
    /// ���Ͱ� �����̴� ��������� �Լ��̴�. 
    /// ����ϰ� �����̴� ���ʹ� ������ �����ʾƵ��ǰ�, ���鸵ó�� Ư���� ������� �����̴� ���ʹ� ������ �ؾ��Ѵ�.
    /// </summary>
    public virtual void MovingBehaviour() { }

    /// <summary>
    /// ���Ͱ� �����̴� ����� ���� ������ �����̰� ���ִ� �Լ�
    /// </summary>
    public abstract void MoveMonster();
    #endregion Move Method

    #region Combat Method
    /// <summary>
    /// �÷��̾��� ������ �¾��� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public abstract void DecreaseHp(float amount);

    /// <summary>
    /// ������ hp�� 0�����϶� ����Ǵ� �Լ�
    /// </summary>
    public abstract void OnMonsterDie();

    /// <summary>
    /// ���Ͱ� ������ �ϱ����� ����Ǵ� �Լ�
    /// </summary>
    public abstract void DoAttack();

    #endregion Combat Method
}
