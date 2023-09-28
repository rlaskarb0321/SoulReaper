using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange_1 : MonsterBase_1, IObjectPooling
{
    [Header("=== Long Range & Object Pool ===")]
    public List<VFXPool> _projectile;
    public Transform _firePos;
    public int _poolCount = 0; // 현재 활성화 되어있는 오브젝트들의 수를 체크

    private bool _needAiming;
    private float _originDelay;
    private readonly int _hashAttack = Animator.StringToHash("Attack");

    protected override void Start()
    {
        base.Start();

        _originDelay = _stat.actDelay;
    }

    private void Update()
    {
        if (_state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_state)
        {
            case eMonsterState.Attack:
                AimingTarget(_target.transform.position, 2.0f);
                Attack();
                break;

            case eMonsterState.Delay:
                SwitchNeedAiming(1);
                Delay();
                AimingTarget(_target.transform.position, 1.0f);
                break;
        }
    }

    public override void Attack()
    {
        if (_animator.GetBool(_hashAttack))
            return;

        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;

        _animator.SetBool(_hashMove, false);
        _animator.SetBool(_hashAttack, true);
    }

    public override void AimingTarget(Vector3 target, float rotMulti)
    {
        if (_needAiming)
        {
            _nav.updatePosition = false;
            Vector3 dir = target - transform.position;
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), _stat.rotSpeed * rotMulti * Time.deltaTime);
        }
        else
        {
            _nav.updatePosition = true;
        }
    }

    public override void SwitchNeedAiming(int value) => _needAiming = value == 1 ? true : false;

    public void ExecuteAtk()
    {
        PullOutObject();
    }

    public void EndAttack()
    {
        _animator.SetBool(_hashAttack, false);
        _state = eMonsterState.Delay;
    }

    public override void Delay()
    {
        if (_stat.actDelay <= 0.0f)
        {
            _stat.actDelay = _originDelay;
            _state = eMonsterState.Trace;
            _needAiming = false;
            return;
        }

        _stat.actDelay -= Time.deltaTime;
    }

    public void PullOutObject()
    {
        if (_poolCount == _projectile.Count)
        {
            AddObject();
        }

        for (int i = 0; i < _projectile.Count; i++)
        {
            if (!_projectile[i].gameObject.activeSelf)
            {
                Vector3 launchAngle = _target.transform.position - transform.position;
                LaunchData launchData = new LaunchData(launchAngle, _firePos.position, _stat.damage, 2.0f);

                _projectile[i].gameObject.SetActive(true);
                _projectile[i].SetPoolData(launchData);
                _poolCount++;
                break;
            }
        }
    }

    public void AddObject()
    {
        VFXPool projectile = Instantiate(_projectile[0], _firePos.position, Quaternion.identity);

        projectile.gameObject.SetActive(false);
        _projectile.Add(projectile);
    }

    public void ReturnObject()
    {
        _poolCount--;
    }
}
