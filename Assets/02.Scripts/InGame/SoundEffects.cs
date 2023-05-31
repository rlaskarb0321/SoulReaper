using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void PlaySFXs(string sfxName, float volume = 1.0f)
    {
        #region Dict로 사운드 캐싱 case 1
        //int i;
        //if (!_sfxDict.ContainsKey(sfxName))
        //{
        //    for (i = 0; i < _sfxs.Length; i++)
        //    {
        //        if (_sfxs[i].name == sfxName)
        //        {
        //            _sfxDict.Add(_sfxs[i].name, i);

        //            _audio.loop = _sfxs[i].isLoop;
        //            _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //            _audio.PlayOneShot(_sfxs[i].sfx);
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    _sfxDict.TryGetValue(sfxName, out i);

        //    _audio.loop = _sfxs[i].isLoop;
        //    _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //    _audio.PlayOneShot(_sfxs[i].sfx);
        //}
        #endregion Dict로 사운드 캐싱

        int i;
        if (_sfxDict.TryGetValue(sfxName, out i))
        {
            _audio.loop = _sfxs[i].isLoop;
            _audio.volume = volume;
            _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
            _audio.PlayOneShot(_sfxs[i].sfx);

            // print("이미 있음");
        }
        else
        {
            for (i = 0; i < _sfxs.Length; i++)
            {
                if (_sfxs[i].name == sfxName)
                {
                    _sfxDict.Add(_sfxs[i].name, i);

                    _audio.loop = _sfxs[i].isLoop;
                    _audio.volume = volume;
                    _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
                    _audio.PlayOneShot(_sfxs[i].sfx);
                    // print("없어서 추가");
                    break;
                }
            }
        }
    }

    // 약간의 딜레이를 가진 후 사운드 실행
    public IEnumerator PlaySFXsDelay(string sfxName, float delay, float volume = 1.0f)
    {
        WaitForSeconds ws = new WaitForSeconds(delay);
        yield return ws;

        PlaySFXs(sfxName, volume);
    }

    // PlayOnAwake가 true인 audioClip 재생
    private void PlayOnAwakeSFX()
    {
        for (int i = 0; i < _sfxs.Length; i++)
        {
            if (_sfxs[i].isPlayOnAwake)
            {
                _audio.PlayOneShot(_sfxs[i].sfx);
            }
        }
    }
}
