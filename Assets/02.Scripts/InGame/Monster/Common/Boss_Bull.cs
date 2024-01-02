using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Bull : WaveMonster
{
    private readonly int _hashRun = Animator.StringToHash("Run");

    protected override void Awake() => base.Awake();

    protected override void Update() => base.Update();

    public override void Trace()
    {
        if (!_monsterBase._nav.enabled)
            return;

        float distance = Vector3.Distance(transform.position, _monsterBase._target.transform.position);

        _monsterBase._animator.SetBool(_hashRun, true);
        if (distance <= _monsterBase._stat.attakDist)
        {
            _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
            _monsterBase._animator.SetBool(_hashRun, false);
        }
        else
        {
            _monsterBase.Move(_monsterBase._target.transform.position, _monsterBase._stat.movSpeed);
        }
    }

    public override GameObject SearchTarget(float searchRange) { return base.SearchTarget(searchRange); }
}
