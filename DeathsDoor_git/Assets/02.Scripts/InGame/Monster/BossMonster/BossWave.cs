using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWave : WaveMonster
{
    protected override void Awake()
    {
        _monsterBase._target = SearchTarget(_monsterBase._stat.traceDist);
    }

    public override GameObject SearchTarget(float searchRange)
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, searchRange, 1 << LayerMask.NameToLayer("PlayerTeam"));
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
        _monsterBase.Move(_monsterBase._target.transform.position, _monsterBase._stat.movSpeed);
    }
}
