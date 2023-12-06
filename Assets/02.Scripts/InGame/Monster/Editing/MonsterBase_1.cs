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
        Idle,   // 정찰 후 포지션을 지킬때
        Scout,
        Trace,
        Attack,
        Delay,  // 공격 후 딜레이 시간을 가질때
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
    public Material[] _hitMats; // 0번 인덱스는 기본 mat, 1번 인덱스는 피격시 잠깐바뀔 mat
    public float _bodyBuryTime; // 시체처리연출의 시작까지 기다릴 값
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
            Debug.LogError(gameObject.name + "오브젝트의 초병 또는 웨이브 등 몬스터 형식을 지정하지 않음");
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
    /// 목표 위치로 movSpeed 값을 가진 속도로 이동함
    /// </summary>
    /// <param name="pos">이동 목표 위치</param>
    /// <param name="movSpeed">목표로 향하는 이동 속도</param>
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
    /// 몬스터의 currHp 를 amount 만큼 깎음
    /// </summary>
    /// <param name="amount">hp를 깎을 양</param>
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
    /// 몬스터가 피격당했을 때, 피격 연출용 코루틴
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
    /// 몬스터가 공격 피격후 currHp = 0 이 되었을 때 호출될 함수
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
    /// 몬스터의 사망 후 연출을 위한 함수
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
    /// 몬스터의 공격 애니메이션 대리자, 타겟을 조준하게하는 메서드이다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rotMulti"></param>
    public virtual void AimingTarget(Vector3 target, float rotMulti) { }

    /// <summary>
    /// 공격을 끝낸 후, 다음 행동에 딜레이를 갖게하는 함수
    /// </summary>
    public virtual void Delay() { }

    /// <summary>
    /// 몬스터의 공격 애니메이션 대리자, 타겟의 조준을 정지시키는 메서드
    /// </summary>
    /// <param name="value"></param>
    public virtual void SwitchNeedAiming(int value) { }
}