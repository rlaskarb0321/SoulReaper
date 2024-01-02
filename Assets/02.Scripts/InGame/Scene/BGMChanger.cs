using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 비지엠을 바꾸기만 하는 클래스, 비지엠 바꿔달라는 요구하는 클래스는 따로 만들어야 한다.
/// </summary>
public class BGMChanger : MonoBehaviour
{
    public AudioClip _originBGM;

    private AudioSource _audio;
    private float _originVolume;
    private bool _isFaded;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        StartCoroutine(DelayBGM());
    }

    private void Start()
    {
        _originVolume = _audio.volume;
    }

    private void Update()
    {
        if (!_isFaded)
        {
            _audio.volume = _originVolume * SettingData._bgmVolume;
        }
    }

    private IEnumerator DelayBGM()
    {
        yield return new WaitForSeconds(0.5f);
        _audio.Play();
    }

    /// <summary>
    /// 원래 비지엠을 바로 꺼주고, 바꿀 비지엠으로 페이드인 하는 메서드
    /// </summary>
    /// <param name="changeBGM"></param>
    public void ChangeDirectly(AudioClip changeBGM, float fadeTime)
    {
        _audio.clip = changeBGM;
        _audio.volume = 0.0f;
        _audio.Play();

        StartCoroutine(FadeInClip(fadeTime));
    }

    /// <summary>
    /// 원래 비지엠을 페이드 아웃으로 낮추고, 바꿀 비지엠으로 천천히 올림
    /// </summary>
    /// <param name="changeBGM"></param>
    /// <param name="fadeTime"></param>
    /// <returns></returns>
    public IEnumerator FadeOutClip(AudioClip changeBGM, float fadeTime)
    {
        float originFadeTime = fadeTime;

        _isFaded = true;
        while (_audio.volume > 0.0f)
        {
            _audio.volume = fadeTime / originFadeTime;
            fadeTime -= Time.deltaTime;
            yield return null;
        }

        if (changeBGM == null)
            yield break;

        _audio.clip = changeBGM;
        _audio.volume = 0.0f;
        _audio.Play();

        StartCoroutine(FadeInClip(originFadeTime));
    }

    private IEnumerator FadeInClip(float fadeTime)
    {
        float temp = 0.0f;
        _isFaded = true;

        while (_audio.volume < _originVolume * SettingData._bgmVolume)
        {
            _audio.volume = temp / fadeTime;
            temp += Time.deltaTime;
            yield return null;
        }
        _isFaded = false;
    }
}
