using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SFXs 
{ 
    public string name; 
    public AudioClip sfx; 
}

public class SceneBGM : MonoBehaviour
{
    public enum eChangeBGM { Normal, FightWithBoss, }
    public SFXs[] _bgmList;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayBGM(eChangeBGM.Normal);
    }

    public void PlayBGM(eChangeBGM bgm)
    {
        _audio.clip = _bgmList[(int)bgm].sfx;
        _audio.loop = true;
        _audio.Play();
    }
}
