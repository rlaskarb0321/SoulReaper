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

    // ���� ���̺꿡 �ִ� ���Ͱ� �� �׾��� ��, ���ݹ��� ���� ���̺갪�� ���������� ����
    public virtual void DecreaseMonsterCount()
    {
        if (--_monsterCount == 0)
        {
            _raidRoom._currWave++;
            _raidRoom.SolveQuest();
        }
    }
}
