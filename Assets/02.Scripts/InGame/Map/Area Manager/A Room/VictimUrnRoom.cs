using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class VictimUrnRoom : QuestRoom
{
    [Header("=== Map ===")]
    [SerializeField] private int _sealCount;
    [SerializeField] private MapTeleport _portal;

    [Header("=== Production ===")]
    [SerializeField] private TimelineAsset _cutScene;
    [SerializeField] private PlayableDirector _playableDirector;

    public override void SolveQuest()
    {
        if (--_sealCount == 0)
        {
            print("solve");
            ProductionMgr.StartProduction(_playableDirector);
            RewardQuest();
        }
    }

    public override void RewardQuest()
    {
        _portal.gameObject.SetActive(true);
    }
}
