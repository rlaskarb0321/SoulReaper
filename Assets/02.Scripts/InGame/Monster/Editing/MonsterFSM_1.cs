using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM_1 : MonoBehaviour
{
    public void ActByFSM(MonsterBase_1.eMonsterState state, MonsterBase_1.eMonsterType type)
    {
        switch (state)
        {
            case MonsterBase_1.eMonsterState.Idle:
                break;
            case MonsterBase_1.eMonsterState.Move:
                break;
            case MonsterBase_1.eMonsterState.Attack:
                break;
            case MonsterBase_1.eMonsterState.Hit:
                break;
            case MonsterBase_1.eMonsterState.Delay:
                break;
            case MonsterBase_1.eMonsterState.Dead:
                break;
            default:
                break;
        }
    }
}
