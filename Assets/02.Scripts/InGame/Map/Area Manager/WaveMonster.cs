using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonster : MonsterType
{
    [Header("=== Wave ===")]
    public RaidWave _waveMaster;

    [Header("=== MonsterBase ===")]
    public MonsterBase_1 _monsterBase;

    private bool _isAlert;

    private void Start()
    {
        _monsterBase._stat.traceDist = 150.0f;
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
        {
            if (_isAlert)
                return;

            AlertDead();
        }

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Trace:
                break;
        }
    }

    // 해당 몬스터가 포함된 웨이브 관리하는 Obj에게 본인이 죽었음을 알림
    public void AlertDead()
    {
        _isAlert = true;
        _waveMaster.DecreaseMonsterCount();
    }
}
