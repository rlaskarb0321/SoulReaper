 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase_1 : MonoBehaviour
{
    public enum eMonsterState
    {
        Idle,   // ���� �� �������� ��ų��
        Scout,
        Trace,
        Attack,
        Delay,  // ���� �� ������ �ð��� ������
        Dead,
    }


    [Header("=== Stat ===")]
    public MonsterStat _stat;
    public float _currHp;

    [Header("=== FSM ===")]
    public eMonsterState _state;

    [Header("=== Target ===")]
    public GameObject _target;

    [Header("=== Hit & Dead ===")]
    public Material[] _hitMats; // 0�� �ε����� �⺻ mat, 1�� �ε����� �ǰݽ� ���ٲ� mat
    public float _bodyBuryTime; // ��üó�������� ���۱��� ��ٸ� ��
    public Material _deadMat;

    [Header("=== Monster Type ===")]
    public MonsterType _monsterType;

    public readonly int _hashMove = Animator.StringToHash("Move");
    public readonly int _hashDead = Animator.StringToHash("Dead");

    [HideInInspector] public NavMeshAgent _nav;
    private SkinnedMeshRenderer _mesh;
    [HideInInspector] public Animator _animator;

    protected virtual void Awake()
    {
        if (_monsterType == null)
        {
            Debug.LogError(gameObject.name + "������Ʈ�� �ʺ� �Ǵ� ���̺� �� ���� ������ �������� ����");
            return;
        }

        _nav = GetComponent<NavMeshAgent>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        _currHp = _stat.health;
    }

    /// <summary>
    /// ��ǥ ��ġ�� movSpeed ���� ���� �ӵ��� �̵���
    /// </summary>
    /// <param name="pos">�̵� ��ǥ ��ġ</param>
    /// <param name="movSpeed">��ǥ�� ���ϴ� �̵� �ӵ�</param>
    public virtual void Move(Vector3 pos, float movSpeed)
    {
        if (_nav.pathPending)
        {
            return;
        }

        _animator.SetBool(_hashMove, true);
        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
    }

    /// <summary>
    /// ������ currHp �� amount ��ŭ ����
    /// </summary>
    /// <param name="amount">hp�� ���� ��</param>
    public virtual void DecreaseHP(float amount)
    {
        if (_currHp <= 0.0f)
            return;

        StartCoroutine(OnHitEvent());
        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
        }
    }

    /// <summary>
    /// ���Ͱ� �ǰݴ����� ��, �ǰ� ����� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 6.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    /// <summary>
    /// ���Ͱ� ���� �ǰ��� currHp = 0 �� �Ǿ��� �� ȣ��� �Լ�
    /// </summary>
    public virtual void Dead()
    {
        GetComponent<CapsuleCollider>().enabled = false;

        _state = eMonsterState.Dead;
        _nav.velocity = Vector3.zero;
        _nav.isStopped = true;
        _nav.baseOffset = 0.0f;
        _nav.enabled = false;
        _animator.SetTrigger(_hashDead);

        StartCoroutine(OnMonsterDead());
    }

    /// <summary>
    /// ������ ��� �� ������ ���� �Լ�
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator OnMonsterDead()
    {
        yield return new WaitForSeconds(_bodyBuryTime);

        Material newMat = Instantiate(_deadMat);
        Color color = newMat.color;

        while (newMat.color.a >= 0.05f)
        {
            color.a -= Time.deltaTime;
            newMat.color = color;
            _mesh.material = newMat;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public virtual void Attack() { }

    /// <summary>
    /// ������ ���� �ִϸ��̼� �븮��, Ÿ���� �����ϰ��ϴ� �޼����̴�.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rotMulti"></param>
    public virtual void AimingTarget(Vector3 target, float rotMulti) { }

    /// <summary>
    /// ������ ���� ��, ���� �ൿ�� �����̸� �����ϴ� �Լ�
    /// </summary>
    public virtual void Delay() { }

    /// <summary>
    /// ������ ���� �ִϸ��̼� �븮��, Ÿ���� ������ ������Ű�� �޼���
    /// </summary>
    /// <param name="value"></param>
    public virtual void SwitchNeedAiming(int value) { }
}