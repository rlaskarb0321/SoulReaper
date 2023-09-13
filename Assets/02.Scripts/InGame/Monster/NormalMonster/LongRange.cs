using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange : MonsterBase, INormalMonster
{
    [Header("=== Launch ===")]
    public GameObject _projectile;
    public Transform _firePos;

    [Header("=== Knock Back ===")]
    public int _knockBackFrame;

    [HideInInspector]
    public MonsterAI _brain;
    private WaitForFixedUpdate _wfs;

    private readonly int _hashAtk1 = Animator.StringToHash("Attack1");
    private readonly int _hashMove = Animator.StringToHash("Move");
    private readonly int _hashIdle = Animator.StringToHash("Idle");
    private readonly int _hashDead = Animator.StringToHash("Dead");

    protected override void Awake()
    {
        base.Awake();

        _brain = GetComponent<MonsterAI>();
        _normalMonster = this;
    }

    protected override void Start()
    {
        base.Start();

        _wfs = new WaitForFixedUpdate();
    }

    public override void DecreaseHp(float amount, Vector3 hitPos)
    {
        StartCoroutine(KnockBack(hitPos));
        StartCoroutine(OnHitEvent());

        _currHp -= amount;
        if (_currHp <= 0.0f)
        {
            _currHp = 0.0f;
            Dead();
        }
    }

    public override IEnumerator OnHitEvent()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;

        // 적 발견하기전에 맞았을 때
        //if (!_brain._isTargetConfirm)
        //{
        //    print("어디서 맞은거지?");
        //    _brain._patrolPos = GameObject.Find("PlayerCharacter").transform.position;
        //}

    }

    public override void Attack()
    {
        _isAtk = true;
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;

        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashAtk1);
    }

    public override void Move(Vector3 pos, float movSpeed)
    {
        if (_nav.pathPending)
        {
            return;
        }

        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
        _animator.SetBool(_hashMove, true);
    }

    public override void Idle()
    {
        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashIdle);
    }

    #region 공격 애니메이션 델리게이트 함수
    public void LaunchMissile() => Instantiate(_projectile, _firePos.position, transform.rotation);

    public void EndAttack()
    {
        _isAtk = !_isAtk;
        _brain._fsm = MonsterAI.eMonsterFSM.Idle;
    }
    #endregion 공격 애니메이션 델리게이트 함수

    public override void LookTarget(Vector3 target, float multiple = 1.0f)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * multiple * Time.deltaTime);
    }

    public IEnumerator KnockBack(Vector3 hitPos)
    {
        hitPos.y = transform.position.y;
        Vector3 knockBackDir = (transform.position - hitPos).normalized;
        int knockBackFrame = _knockBackFrame;

        while (--knockBackFrame >= 0)
        {
            transform.position += knockBackDir * Time.fixedDeltaTime;
            yield return _wfs;
        }
    }

    protected override void Dead()
    {
        Collider coll = this.GetComponent<Collider>();

        _brain._fsm = MonsterAI.eMonsterFSM.Dead;
        _animator.SetTrigger(_hashDead);
        _nav.velocity = Vector3.zero;
        _nav.isStopped = true;
        _nav.baseOffset = 0.0f;
        coll.enabled = false;

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
}
