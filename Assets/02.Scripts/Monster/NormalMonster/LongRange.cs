using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange : MonsterBase
{
    [Space(15.0f)]
    public GameObject _projectile;
    public Transform _firePos;

    private readonly int _hashAtk1 = Animator.StringToHash("Attack1");
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void DecreaseHp(float amount)
    {
        _currHp -= amount;
        StartCoroutine(OnHitEvent());
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

        _animator.SetBool(_hashMove, true);
        _nav.isStopped = false;
        _nav.speed = movSpeed;
        _nav.SetDestination(pos);
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

    public override void LookTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * Time.deltaTime);
    }

    protected override void Dead()
    {
        BoxCollider boxColl = this.GetComponent<BoxCollider>();

        _brain._fsm = MonsterAI.eMonsterFSM.Dead;
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
