using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierPigeon : MonoBehaviour
{
    [SerializeField]
    private AudioClip _flaySparrowSound;

    private Animator _animator;
    private AudioSource _audio;
    private SoundEffects _sfx;

    private readonly int _hashFly = Animator.StringToHash("fly");

    private void Awake()
    {
        _sfx = GetComponent<SoundEffects>();
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    public void FlyAway()
    {
        _animator.SetBool(_hashFly, true);
    }

    public void PlayFlySound()
    {
        _audio.PlayOneShot(_flaySparrowSound, _audio.volume * SettingData._sfxVolume);
    }

    public void TurnOff() => gameObject.SetActive(false);
}
