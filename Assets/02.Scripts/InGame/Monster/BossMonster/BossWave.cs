using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWave : WaveMonster
{
    protected override void Awake()
    {
        _monsterBase._target = SearchTarget();
    }

    public override GameObject SearchTarget()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _monsterBase._stat.traceDist, 1 << LayerMask.NameToLayer("PlayerTeam"));
        for (int i = 0; i < colls.Length; i++)
        {
            if (!colls[i].CompareTag("Player"))
                continue;

            return colls[i].gameObject;
        }

        return null;
    }

    public override void Trace()
    {

    }
}
