using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2RoomMgr : QuestRoom
{
    public Brazier[] _braziers;
    public Brazier _solveObj;
    public Key _reward;

    public override void SolveQuest()
    {
        if (_solveObj._brazierState == Brazier.eBrazier.Fire)
        {
            _reward.Reward();
            return;
        }
    }
}
