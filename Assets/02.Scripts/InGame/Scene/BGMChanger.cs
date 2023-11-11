using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 비지엠을 바꾸기만 하는 클래스, 비지엠 바꿔달라는 요구하는 클래스는 따로 만들어야 한다.
/// </summary>
public class BGMChanger : MonoBehaviour
{
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        StartCoroutine(DelayBGM());
    }

    private IEnumerator DelayBGM()
    {
        yield return new WaitForSeconds(0.5f);
        _audio.Play();
    }

    /// <summary>
    /// 비지엠을 바꿔주는 메서드
    /// </summary>
    /// <param name="changeBGM"></param>
    public void ChangeDirectly(AudioClip changeBGM, float fadeTime)
    {
        _audio.clip = changeBGM;
        _audio.volume = 0.0f;
        _audio.Play();

        StartCoroutine(FadeInClip(fadeTime));
    }

    public IEnumerator FadeOutClip(AudioClip changeBGM, float fadeTime)
    {
        float framePerFadeTime = fadeTime / Time.deltaTime;
        float amount = 1.0f / framePerFadeTime;

        while (_audio.volume > 0.0f)
        {
            _audio.volume -= amount;
            yield return null;
        }

        _audio.clip = changeBGM;
        _audio.volume = 0.0f;
        _audio.Play();

        StartCoroutine(FadeInClip(fadeTime));
    }

    private IEnumerator FadeInClip(float fadeTime)
    {
        float framePerFadeTime = fadeTime / Time.deltaTime;
        float amount = 1.0f / framePerFadeTime;

        while (_audio.volume < 1.0f)
        {
            _audio.volume += amount;
            yield return null;
        }
    }
}
