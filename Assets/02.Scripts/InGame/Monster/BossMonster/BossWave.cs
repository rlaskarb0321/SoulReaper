using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWave : WaveMonster
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override GameObject SearchTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _monsterBase._stat.traceDist, 1 << LayerMask.NameToLayer("PlayerTeam"));

        return colls[0].gameObject;
    }

    public override void Trace()
    {

    }
}
