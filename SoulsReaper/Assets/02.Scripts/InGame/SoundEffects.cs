using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트가 낼 수 있는 audioClip, 그 외 정보를 묶어주는 구조체
[System.Serializable]
public struct SFXs
{
    public string name;
    public AudioClip sfx;
    public bool isLoop;
    public bool isPlayOnAwake;
}

// bgm과 ui 관련 효과음을 제외한, 인 게임내 오브젝트들의 효과음 관련 스크립트

public class SoundEffects : MonoBehaviour
{
    public SFXs[] _sfxs;

    private AudioSource _audio;
    private Dictionary<string, int> _sfxDict;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _sfxDict = new Dictionary<string, int>();

        PlayOnAwakeSFX();
    }

    // 타 스크립트에서 이 스크립트를 가질 게임오브젝트 소리를 실행시키기 위한 함수
    public void PlayOneShotUsingDict(string sfxName, float volume = 1.0f)
    {
        if (volume == 1.0f)
            volume = _audio.volume;

        int i = ReturnSFXIndex(sfxName);
        if (i == -1)
        {
            print("sfxName 값이 sfxs 에 없습니다");
            return;
        }

        _audio.loop = _sfxs[i].isLoop;
        _audio.volume = volume;
        _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        _audio.PlayOneShot(_sfxs[i].sfx, _audio.volume * SettingData._sfxVolume);

        #region 23.10.17 Dict 작업 메서드로 분리
        //int i;
        //// Dict 에 sfxName 으로 된 키값이 있을경우
        //if (_sfxDict.TryGetValue(sfxName, out i))
        //{
        //    _audio.loop = _sfxs[i].isLoop;
        //    _audio.volume = volume;
        //    _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //    _audio.PlayOneShot(_sfxs[i].sfx);
        //}
        //// 없을경우
        //else
        //{
        //    // 입력된 sfxName값이, 해당 객체가 플레이할 수 있는 clip 명인지 확인후 Dict에 추가
        //    for (i = 0; i < _sfxs.Length; i++)
        //    {
        //        // 일치함을 확인
        //        if (_sfxs[i].name == sfxName)
        //        {
        //            _sfxDict.Add(_sfxs[i].name, i);

        //            _audio.loop = _sfxs[i].isLoop;
        //            _audio.volume = volume;
        //            _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //            _audio.PlayOneShot(_sfxs[i].sfx);
        //            break;
        //        }
        //    }
        //}
        #endregion 23.10.17 Dict 작업 메서드로 분리
    }

    public void PlayUsingDict(string sfxName, float volume = 1.0f)
    {
        if (volume == 1.0f)
            volume = _audio.volume;

        int i = ReturnSFXIndex(sfxName);
        if (i == -1)
        {
            print("sfxName 값이 sfxs 에 없습니다");
            return;
        }

        _audio.loop = _sfxs[i].isLoop;
        _audio.volume = volume;
        _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        _audio.clip = _sfxs[i].sfx;
        _audio.Play();
    }

    public void Stop() => _audio.Stop();

    public bool IsPlaying()
    {
        return _audio.isPlaying;
    }

    public int ReturnSFXIndex(string sfxName)
    {
        int i;

        if(_sfxDict.TryGetValue(sfxName, out i))
        {
            return i;
        }
        else
        {
            for (i = 0; i < _sfxs.Length; i++)
            {
                if (_sfxs[i].name == sfxName)
                {
                    _sfxDict.Add(_sfxs[i].name, i);
                    return i;
                }
            }

            return -1;
        }
    }

    // 약간의 딜레이를 가진 후 사운드 실행
    public IEnumerator PlaySFXsDelay(string sfxName, float delay, float volume = 1.0f)
    {
        WaitForSeconds ws = new WaitForSeconds(delay);
        yield return ws;

        PlayOneShotUsingDict(sfxName, volume);
    }

    // PlayOnAwake가 true인 audioClip 재생
    private void PlayOnAwakeSFX()
    {
        for (int i = 0; i < _sfxs.Length; i++)
        {
            if (_sfxs[i].isPlayOnAwake)
            {
                _audio.PlayOneShot(_sfxs[i].sfx, _audio.volume * SettingData._sfxVolume);
            }
        }
    }
}
