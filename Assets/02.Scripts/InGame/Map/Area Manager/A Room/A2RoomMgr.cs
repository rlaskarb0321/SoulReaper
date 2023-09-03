using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A2RoomMgr : MonoBehaviour
{
    public Brazier[] _braziers;
    public Brazier _solveObj;
    public Key _reward;

    public void ProceedingPuzzle()
    {
        if (_solveObj._brazierState == Brazier.eBrazier.Fire)
        {
            RewardsPuzzle();
            return;
        }

        //print("fire");
    }

    private void RewardsPuzzle()
    {
        _reward.SolveReward();
    }
}
