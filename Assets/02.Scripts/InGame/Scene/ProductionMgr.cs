using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ProductionMgr : MonoBehaviour
{
    public static PlayableDirector _director;
    public static bool _isPlayingProduction;

    private static bool _isCallProduction;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (!_isCallProduction)
            return;

        _isPlayingProduction = IsPlayingCutScene();
    }

    public static void StartProduction(PlayableDirector playable)
    {
        _isCallProduction = true;

        _director = playable;
        _director.Play();
    }

    public static void StopProduction(PlayableDirector playable)
    {
        _isCallProduction = false;
        _isPlayingProduction = false;

        _director = playable;
        _director.Stop();
    }

    private bool IsPlayingCutScene()
    {
        if (System.Math.Abs(_director.duration - _director.time) < 0.05f)
        {
            _isCallProduction = false;
            return false;
        }
        else
        {
            return true;
        }
    }
}
