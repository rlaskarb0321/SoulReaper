using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    [Header("=== Bgm Fade ===")]
    [SerializeField]
    private float _fadeTime;

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

    public void ChangeBGM(AudioClip changeBGM)
    {
        _audio.clip = changeBGM;
        _audio.volume = 0.0f;
        _audio.Play();

        StartCoroutine(FadeClip());
    }

    private IEnumerator FadeClip()
    {
        float framePerFadeTime = _fadeTime / Time.deltaTime;
        float amount = 1.0f / framePerFadeTime;

        while (_audio.volume < 1.0f)
        {
            _audio.volume += amount;
            yield return null;
        }
    }
}
