using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2RoomMgr : QuestRoom
{
    public Flammable[] _braziers;
    public Flammable _solveObj;
    public Key _reward;

    public override void SolveQuest()
    {
        if (_solveObj._fireState == Flammable.eFireState.Fire)
        {
            _reward.Reward();
            return;
        }
    }
}
