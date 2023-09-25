using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonster : MonsterType
{
    [Header("=== Wave ===")]
    public RaidWave _waveMaster;

    [Header("=== MonsterBase ===")]
    public MonsterBase_1 _monsterBase;

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        switch (_monsterBase._state)
        {
            case MonsterBase_1.eMonsterState.Trace:
                break;
        }
    }

    // �ش� ���Ͱ� ���Ե� ���̺� �����ϴ� Obj���� ������ �׾����� �˸�
    public void AlertDead()
    {
        _waveMaster.DecreaseMonsterCount();
    }


}
