using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidRoom : QuestRoom
{
    // 잡몹, 미니보스들의 공세가 시작되는 방
    // 1. 비지엠 바꾸기

    [Header("=== Alarm ===")]
    public GameObject _entranceBlockObj;
    public GameObject _ladder;

    [Header("=== Raid Room ===")]
    public RaidWave[] _waves;
    public MonsterBase_1[] _monsters;
    public int _currWave = 0;

    // 웨이브 격퇴를 기록, 모든 웨이브가 격퇴되면 해당 방의 퀘스트 완료
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
