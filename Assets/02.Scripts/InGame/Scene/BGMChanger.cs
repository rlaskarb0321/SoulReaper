using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� �ٲٱ⸸ �ϴ� Ŭ����, ������ �ٲ�޶�� �䱸�ϴ� Ŭ������ ���� ������ �Ѵ�.
/// </summary>
public class BGMChanger : MonoBehaviour
{
    private AudioSource _audio;
    private float _originVolume;

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
        _audio.volume = _originVolume * SettingData._bgmVolume;
    }

    private IEnumerator DelayBGM()
    {
        yield return new WaitForSeconds(0.5f);
        _audio.Play();
    }

    /// <summary>
    /// �������� �ٲ��ִ� �޼���
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
        float originFadeTime = fadeTime;
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

        StartCoroutine(FadeInClip(originFadeTime * 0.5f));
    }

    private IEnumerator FadeInClip(float fadeTime)
    {
        float temp = 0.0f;
        while (_audio.volume < _originVolume * SettingData._bgmVolume)
        {
            _audio.volume = temp / fadeTime;
            temp += Time.deltaTime;
            yield return null;
        }
    }
}
