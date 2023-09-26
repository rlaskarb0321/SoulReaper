using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange_1 : MonsterBase_1
{
    [Header("=== Long Range ===")]
    public GameObject _projectile;
    public Transform _firePos;

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
                break;
            case eMonsterState.Delay:
                break;
            case eMonsterState.Dead:
                break;
        }
    }
}
