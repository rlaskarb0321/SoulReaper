using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ProductionMgr : MonoBehaviour
{
    public static PlayableDirector _director;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    public static void StartProduction(PlayableDirector playable)
    {
        _director = playable;
        _director.Play();
    }
}
