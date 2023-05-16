using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRange_2 : MonsterBase_2
{
    [Space(15.0f)]
    public GameObject _atkCollObj;
    public float _atkDmg;

    private BoxCollider _atkColl;
    private readonly int _hashAtk1 = Animator.StringToHash("Attack1");

    protected override void Awake()
    {
        base.Awake();

        _atkColl = _atkCollObj.GetComponent<BoxCollider>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        _isAtk = true;
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;
        _animator.SetTrigger(_hashAtk1);
    }

    #region 공격애니메이션 델리게이트 함수
    public void ExecuteAtk() => _atkColl.enabled = !_atkColl.enabled;
    #endregion 공격애니메이션 델리게이트 함수

    public override void DecreaseHp(float amount)
    {
        _currHp -= amount;
        StartCoroutine(OnHitEffect());
        if (_currHp <= 0.0f)
        {
            Dead();
        }
    }

    public override void Idle()
    {
        _animator.SetBool(_hashMove, false);
        _animator.SetTrigger(_hashIdle);
    }

    public override void LookTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * Time.deltaTime);
    }

    public override void Move(Vector3 pos, float movSpeed)
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

    public override IEnumerator OnHitEffect()
    {
        Material newMat;

        newMat = _hitMats[1];
        _mesh.material = newMat;
        yield return new WaitForSeconds(Time.deltaTime * 4.0f);

        newMat = _hitMats[0];
        _mesh.material = newMat;
    }

    protected override void Dead()
    {
        BoxCollider boxColl = this.GetComponent<BoxCollider>();

        _brain._fsm = MonsterAI_2.eMonsterFSM.Dead;
        _animator.SetTrigger(_hashDead);
        _nav.velocity = Vector3.zero;
        _nav.isStopped = true;
        _nav.baseOffset = 0.0f;
        boxColl.size = Vector3.one;
        boxColl.enabled = false;

        StartCoroutine(OnMonsterDie());
    }

    protected override IEnumerator OnMonsterDie()
    {
        yield return new WaitForSeconds(_bodyBuryTime);

        Material newMat = _deadMat[1];
        newMat.color = Vector4.one;
        Color color = newMat.color;

        while (newMat.color.a >= 0.05f)
        {
            color.a -= Time.deltaTime;
            newMat.color = color;
            _mesh.material = newMat;
            yield return null;
        }
        this.gameObject.SetActive(false);
    }
}
