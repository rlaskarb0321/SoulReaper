using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public SFXs[] _sfxs;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _audio.playOnAwake = false;
        _audio.loop = false;
    }

    // �Ҹ��� �����Ű������ ȣ���ϴ� �Լ�
    public void PlaySFXs(int idx = 0)
    {
        _audio.PlayOneShot(_sfxs[idx].sfx);
    }
}
