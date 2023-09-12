using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bull : MonsterBase
{
    public enum eBossState { Idle, Move, Attack, Delay, Dead, }

    [Header("=== Miniboss Bull ===")]
    public Transform _target;
    public float _attackRange;
    public SphereCollider[] _weaponColl;
    public eBossState _state;

    // MonsterAI 사용하는 스크립트
    //private MonsterAI _brain;
    //private int _comboCount;

    private float _currDelay;
    private bool _needTrace;

    private readonly int _hashIsDead = Animator.StringToHash("isDead");
    private readonly int _hashAttack = Animator.StringToHash("Attack");
    private readonly int _hashAtkCombo = Animator.StringToHash("AtkCombo");

    protected override void Awake()
    {
        base.Awake();

        // MonsterAI 사용하는 스크립트
        //_brain = GetComponent<MonsterAI>();
    }

    protected override void Start()
    {
        base.Start();

        _state = eBossState.Move;
        _currDelay = _stat.actDelay;
    }

    private void Update()
    {
        switch (_state)
        {
            case eBossState.Move:
                bool canAttack = TargetNearbyRange(_target, _stat.attakDist);
                if (canAttack)
                {
                    _state = eBossState.Attack;
                }
                else
                {
                    Move(_target.position, _stat.movSpeed);
                }
                break;

            case eBossState.Attack:
                if (_needTrace)
                {
                    _nav.updatePosition = false;
                    LookTarget(_target.position);
                }
                else
                {
                    _nav.updatePosition = true;
                }

                Attack();
                break;

            case eBossState.Delay:
                bool targetNearAttackDist = TargetNearbyRange(_target, _stat.attakDist);
                if (_currDelay <= 0.0f)
                {
                    _currDelay = _stat.actDelay;
                    _animator.SetBool(_hashAttack, false);
                    _state = targetNearAttackDist ? eBossState.Attack : eBossState.Move;
                    return;
                }

                LookTarget(_target.position);
                _currDelay -= Time.deltaTime;
                break;

            case eBossState.Dead:
                return;
        }
    }

    // 콤보를 이어 갈 수 있다면 다음 콤보를 실행, 마지막 콤보에 다다르거나 콤보가 불가능하면 공격 취소
    // 공격 애니메이션에 붙히는 델리게이트 함수
    public void SearchTarget()
    {
        bool canCombo = TargetNearbyRange(_target, _stat.attakDist * 1.75f);
        int combo = 0;

        if (canCombo)
        {
            combo = _animator.GetInteger(_hashAtkCombo);
            combo++;
            combo %= 3;
        }

        if (combo == 0 || !canCombo)
        {
            _state = eBossState.Delay;
            _animator.SetBool(_hashAttack, false);
        }

        _animator.SetInteger(_hashAtkCombo, combo);
    }

    public override void Attack()
    {
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;
        _animator.SetBool(_hashAttack, true);
    }

    // 공격 애니메이션에서 적을 바라보는 기능 키고 끄기용 델리게이트 메서드
    public void SwitchNeedTrace(int value) => _needTrace = value == 1 ? true : false; 

    public override void DecreaseHp(float amount, Vector3 hitPos)
    {
        StartCoroutine(OnHitEvent());

        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            GetComponent<CapsuleCollider>().enabled = false;
            _currHp = 0.0f;
            _animator.SetTrigger(_hashIsDead);
            _state = eBossState.Dead;
        }
    }

    public override void Idle()
    {

    }

    public override void Move(Vector3 pos, float movSpeed)
    {
        if (_nav.pathPending)
        {
            return;
        }

        if (Vector3.Distance(transform.position, _target.position) <= _nav.stoppingDistance)
        {
            return;
        }

        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
    }

    public override void LookTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * Time.deltaTime);
    }

    public override IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 2.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    protected override void Dead()
    {
        StartCoroutine(OnMonsterDie());
    }

    protected override IEnumerator OnMonsterDie()
    {
        yield return new WaitForSeconds(_bodyBuryTime);

        //Material newMat = _mesh.material;
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

    

    public bool TargetNearbyRange(Transform target, float range)
    {
        return Vector3.Distance(target.position, transform.position) <= range;
    }
}
