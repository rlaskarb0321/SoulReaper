 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase_1 : MonoBehaviour
{
    public enum eMonsterState
    {
        Idle,   // 정찰 후 포지션을 지킬때
        Scout,
        Trace,
        Attack,
        GetHit,
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
    public WaveMonster _waveMonster;
    public SentryMonster_1 _sentryMonster;

    [HideInInspector] public NavMeshAgent _nav;
    private SkinnedMeshRenderer _mesh;

    protected void Awake()
    {
        if (_waveMonster == null && _sentryMonster == null)
        {
            Debug.LogError(gameObject.name + "오브젝트의 초병 또는 웨이브 등 몬스터 형식을 지정하지 않음");
            return;
        }

        if ((_waveMonster != null && _sentryMonster != null))
        {
            Debug.LogError(gameObject.name + "오브젝트의 초병 또는 웨이브 등 몬스터 형식은 하나만 지정해야 함");
            return;
        }

        if (_waveMonster != null)
        {
            _stat.traceDist = 150.0f;
        }

        _nav = GetComponent<NavMeshAgent>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    protected void Start()
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
        if (_nav.pathPending)
        {
            return;
        }

        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
    }

    /// <summary>
    /// 몬스터의 currHp 를 amount 만큼 깎음
    /// </summary>
    /// <param name="amount">hp를 깎을 양</param>
    public virtual void DecreaseHP(float amount)
    {
        StartCoroutine(OnHitEvent());

        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
            if (_waveMonster != null)
            {
                _waveMonster.AlertDead();
            }
        }
    }

    /// <summary>
    /// 몬스터가 피격당했을 때, 피격 연출용 코루틴
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    /// <summary>
    /// 몬스터가 공격 피격후 currHp = 0 이 되었을 때 호출될 함수
    /// </summary>
    public virtual void Dead()
    {
        GetComponent<BoxCollider>().enabled = false;

        _nav.velocity = Vector3.zero;
        _nav.isStopped = true;
        _nav.baseOffset = 0.0f;

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
    }

    public virtual void Attack() { }
}