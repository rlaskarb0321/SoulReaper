using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonster : MonoBehaviour
{
    public RaidWave _waveMaster;

    // 해당 몬스터가 포함된 웨이브 관리하는 Obj에게 본인이 죽었음을 알림
    public void AlertDead()
    {
        _waveMaster.DecreaseMonsterCount();
    }
}
