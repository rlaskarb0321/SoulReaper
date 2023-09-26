using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange_1 : MonsterBase_1
{
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
