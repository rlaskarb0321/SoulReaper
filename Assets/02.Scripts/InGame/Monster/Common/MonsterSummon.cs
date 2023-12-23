using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� ��ȯ�Ҷ� ���̴� ������ ���Ǵ� ��ũ��Ʈ
/// </summary>
public class MonsterSummon : MonoBehaviour
{
    public GameObject _summonMonster;
    public GameObject _aura;
    public AudioClip[] _soundClip;

    private enum eSoundClip { SummonStart, SummonLoop, SummonSuccess, SummonFail }
    private AudioSource _audio;
    private Animator _animator;
    private ISummonType _summonType; // SummonMonster ��ü�� �ִ� ISummonType �������̽��� �Ҵ�

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _summonType = _summonMonster.GetComponent<ISummonType>();
    }

    /// <summary>
    /// �ܺο��� ��ȯ ������ �� ȣ���ϴ� ��ũ��Ʈ
    /// </summary>
    public void StartSummon()
    {
        _summonType.InitUnitData();
        _aura.gameObject.SetActive(true);

        _animator.enabled = false;
        _animator.Rebind();
        _animator.enabled = true;
    }

    public void SetMonsterAIOn()
    {
        _summonType.CompleteSummon();
    }

    public void SetMonsterOff(int value = 0)
    {
        bool isOn = value == 1 ? true : false;
        _summonMonster.gameObject.SetActive(isOn);
    }

    public void CrescSound() => StartCoroutine(FadeInSound());

    public void DecrescSound() => StartCoroutine(FadeOutSound());

    private IEnumerator FadeOutSound()
    {
        _audio.volume = 1.0f;
        while (_audio.volume > 0.0f)
        {
            _audio.volume -= Time.deltaTime * 0.5f;
            yield return null;
        }
        _audio.volume = 0.0f;
    }

    private IEnumerator FadeInSound()
    {
        _audio.volume = 0.0f;
        while (_audio.volume < 1.0f)
        {
            _audio.volume += Time.deltaTime * 0.5f;
            yield return null;
        }
        _audio.volume = 1.0f;
    }

    public void PlayAuraSound(int index)
    {
        if (index == (int)eSoundClip.SummonLoop && !_audio.isPlaying)
        {
            _audio.clip = _soundClip[index];
            _audio.loop = true;
            _audio.Play();
            return;
        }

        _audio.volume = 1.0f;
        _audio.loop = false;
        _audio.Stop();
        _audio.PlayOneShot(_soundClip[index], _audio.volume * SettingData._sfxVolume);
    }
}
