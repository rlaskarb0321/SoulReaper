using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidRoom : QuestRoom
{
    // ���, �̴Ϻ������� ������ ���۵Ǵ� ��
    // 1. ������ �ٲٱ�

    [Header("=== Alarm ===")]
    public GameObject _entranceBlockObj;
    public GameObject _ladder;

    [Header("=== Raid Room ===")]
    public RaidWave[] _waves;
    public MonsterBase_1[] _monsters;
    public int _currWave = 0;

    // ���̺� ���� ���, ��� ���̺갡 ����Ǹ� �ش� ���� ����Ʈ �Ϸ�
    public override void SolveQuest()
    {
        if (_currWave >= _waves.Length)
        {
            _ladder.gameObject.SetActive(true);
            _entranceBlockObj.SetActive(false);
            return;
        }

        _waves[_currWave].gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
        {
            return;
        }

        _entranceBlockObj.SetActive(true);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        SolveQuest();
    }
}
