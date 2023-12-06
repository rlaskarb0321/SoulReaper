 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IDotDebuff
{
    public IEnumerator DecreaseDebuffDur(BurnDotDamage dotDamage);

    public void DotDamaged();
}

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

    [Header("=== Sound Clip ===")]
    [SerializeField]
    protected AudioClip[] _sound;

    protected enum eSound { Attack, Die }

    public readonly int _hashMove = Animator.StringToHash("Move");
    public readonly int _hashDead = Animator.StringToHash("Dead");
    public readonly int _hashRevive = Animator.StringToHash("Revive");
    [HideInInspector] public NavMeshAgent _nav;
    [HideInInspector] public SkinnedMeshRenderer _mesh;
    [HideInInspector] public Animator _animator;
    [HideInInspector] public NavMeshPath _path;
    protected AudioSource _audio;


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
        _audio = GetComponent<AudioSource>();
        _path = new NavMeshPath();
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
        if (!_nav.enabled)
            return;
        if (_nav.pathPending)
            return;

        _animator.SetBool(_hashMove, true);
        _nav.isStopped = false;
        _nav.speed = movSpeed;

        //_nav.CalculatePath(pos, _path);
        //print(_path.status == NavMeshPathStatus.PathPartial);

        _nav.SetDestination(pos);
    }

    public bool IsPathPartial(Vector3 pos)
    {
        _nav.CalculatePath(pos, _path);
        return _path.status == NavMeshPathStatus.PathPartial;
    }

    /// <summary>
    /// ������ currHp �� amount ��ŭ ����
    /// </summary>
    /// <param name="amount">hp�� ���� ��</param>
    public virtual void DecreaseHP(float amount, BurnDotDamage burn = null)
    {
        if (_currHp <= 0.0f)
            return;

        // StartCoroutine(OnHitEvent());
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
    public virtual IEnumerator OnHitEvent(eArrowState state = eArrowState.Normal)
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
        if (_sound.Length > (int)eSound.Die)
        {
            _audio.PlayOneShot(_sound[(int)eSound.Die]);
        }

        UIScene._instance.UpdateSoulCount(_stat.soul);
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
        _animator.enabled = true;
        _animator.SetTrigger(_hashRevive);
        _animator.enabled = false;
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