using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidWave : MonoBehaviour
{
    public RaidRoom _raidRoom;
    public int _waveIndex;
    public int _monsterCount;
    public WaveMonster[] _monsters;

    private void Awake()
    {
        _monsterCount = transform.childCount;
    }

    // 본인 웨이브에 있는 몬스터가 다 죽었을 때, 습격방의 현재 웨이브값을 다음값으로 갱신
    public virtual void DecreaseMonsterCount()
    {
        if (--_monsterCount == 0)
        {
            _raidRoom._currWave++;
            _raidRoom.SolveQuest();
        }
    }
}
