using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������Ʈ�� �� �� �ִ� audioClip, �� �� ������ �����ִ� ����ü
[System.Serializable]
public struct SFXs 
{ 
    public string name; 
    public AudioClip sfx;
    public bool isLoop;
    public bool isPlayOnAwake;
}

public class SceneBGM : MonoBehaviour
{
    public enum eGameBGM { Main, Boss }
    public eGameBGM _gameBGM;
    public SFXs[] _bgmList;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PlayBGM(eGameBGM.Main);
    }

    public void PlayBGM(eGameBGM bgmType)
    {
        _audio.clip = _bgmList[(int)bgmType].sfx;
        _audio.playOnAwake = _bgmList[(int)bgmType].isPlayOnAwake;
        _audio.loop = _bgmList[(int)bgmType].isLoop;

        _audio.Play();
    }
}
