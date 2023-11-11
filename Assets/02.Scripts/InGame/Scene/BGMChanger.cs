using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� �ٲٱ⸸ �ϴ� Ŭ����, ������ �ٲ�޶�� �䱸�ϴ� Ŭ������ ���� ������ �Ѵ�.
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
