using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A3RoomMgr : QuestRoom
{
    [SerializeField] private int _sealCount;
    [SerializeField] private MapTeleport _portal_A4;

    public override void SolveQuest()
    {
        if (--_sealCount == 0)
        {
            print("solve");
            RewardQuest();
        }
    }

    public override void RewardQuest()
    {
        _portal_A4.gameObject.SetActive(true);
    }
}
