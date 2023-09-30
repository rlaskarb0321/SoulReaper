using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonsterBase_1 �� ���
public class Bull_1 : MeleeRange
{
    public SphereCollider[] _weaponColl;

    private readonly int _hashAtkCombo = Animator.StringToHash("AtkCombo");
    private readonly int _hashIdle = Animator.StringToHash("Idle");
    

    protected override void Start() => base.Start();

    protected override void Update()
    {
        if (_state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_state)
        {
            case eMonsterState.Attack:
                AimingTarget(_target.transform.position, 2.0f);
                Attack();
                break;

            // ���� �� ��Ÿ�� ������ �ð�, ��Ÿ���� ���� ���� �÷��̾ ������������� ���ο����� �ִϸ��̼� ���°� �ٸ��� ���̵ȴ�.
            case eMonsterState.Delay:
                bool targetNearAttackDist = TargetNearbyRange(_target.transform, _stat.attakDist);

                if (_stat.actDelay <= 0.0f)
                {
                    _stat.actDelay = _originDelay;
                    _animator.SetBool(_hashAttack, false);
                    _state = targetNearAttackDist ? eMonsterState.Attack : eMonsterState.Trace;
                    return;
                }
                _stat.actDelay -= Time.deltaTime;
                _animator.SetBool(_hashIdle, targetNearAttackDist);
                _animator.SetBool(_hashMove, !targetNearAttackDist);

                if (!targetNearAttackDist)
                {
                    Move(_target.transform.position, _stat.movSpeed * 0.2f);
                }
                break;
        }
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
    }

    public override void Attack()
    {
        _nav.isStopped = true;
        _nav.velocity = Vector3.zero;
        _animator.SetBool(_hashAttack, true);
    }

    public override void SwitchNeedAiming(int value) => base.SwitchNeedAiming(value);

    public void ExecuteAttack(int weaponIndex)
    {
        if (weaponIndex == 2)
        {
            for (int i = 0; i < _weaponColl.Length; i++)
            {
                _weaponColl[i].enabled = !_weaponColl[i].enabled;
            }

            return;
        }

        _weaponColl[weaponIndex].enabled = !_weaponColl[weaponIndex].enabled;
    }

    /// <summary>
    // �޺��� �̾� �� �� �ִٸ� ���� �޺��� ����, ������ �޺��� �ٴٸ��ų� �޺��� �Ұ����ϸ� ���� ���, ���� �ִϸ��̼ǿ� ������ ��������Ʈ �Լ�
    /// </summary>
    public void MakingComboAttack()
    {
        bool canCombo = TargetNearbyRange(_target.transform, _stat.attakDist * 1.75f);
        int combo = 0;

        if (canCombo)
        {
            combo = _animator.GetInteger(_hashAtkCombo);
            combo++;
            combo %= 3;
        }

        if (combo == 0 || !canCombo)
        {
            _state = eMonsterState.Delay;
            _animator.SetBool(_hashAttack, false);
        }

        _animator.SetInteger(_hashAtkCombo, combo);
    }

    public bool TargetNearbyRange(Transform target, float range)
    {
        return Vector3.Distance(target.position, transform.position) <= range;
    }

    // �������Ͱ� �׾��� �� ���� �ݸ��� �� ���� ������ ����
    public override void Dead()
    {
        for (int i = 0; i < _weaponColl.Length; i++)
        {
            _weaponColl[i].enabled = false;
        }

        GetComponent<BoxCollider>().enabled = false;
        _nav.enabled = false;
        _currHp = 0.0f;
        _animator.SetTrigger(_hashDead);
        _state = eMonsterState.Dead;

        StartCoroutine(OnMonsterDead());
    }
}
