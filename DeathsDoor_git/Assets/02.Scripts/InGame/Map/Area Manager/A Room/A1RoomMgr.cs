using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1RoomMgr : QuestRoom
{
    public MapTeleport _shrinePortal;
    public Animator _shelf;

    public override void SolveQuest()
    {
        _shelf.enabled = true;
        _shrinePortal.gameObject.SetActive(true);
    }
}
