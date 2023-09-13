using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A4RoomMgr : QuestRoom
{
    public GameObject _entranceBlockObj;
    public Bull _bull;
    public GameObject _ladder;

    public override void RewardQuest()
    {

    }

    public override void SolveQuest()
    {
        GetComponent<BoxCollider>().enabled = false;
        _entranceBlockObj.SetActive(false);
        _ladder.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        _entranceBlockObj.SetActive(true);
        _bull.gameObject.SetActive(true);
    }
}
