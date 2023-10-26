using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class VictimUrnRoom : QuestRoom
{
    [Header("=== Map ===")]
    public int _sealCount;
    [SerializeField] private MapTeleport _portal;

    [Header("=== Production ===")]
    [SerializeField] private TimelineAsset _cutScene;
    [SerializeField] private PlayableDirector _playableDirector;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _apply;

    public override void SolveQuest()
    {
        if (--_sealCount == 0)
        {
            ProductionMgr.StartProduction(_playableDirector);
            _portal.gameObject.SetActive(true);

            if (_apply != null)
                _apply.EditMapData();
        }
    }
}
