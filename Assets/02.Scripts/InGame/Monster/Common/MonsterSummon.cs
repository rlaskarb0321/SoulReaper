using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 누군가가 소환할때 쓰이는 오오라에 사용되는 스크립트
/// </summary>
public class MonsterSummon : MonoBehaviour
{
    public GameObject _summonMonster;
    public AudioClip[] _soundClip;

    private enum eSoundClip { SummonStart, SummonLoop, SummonSuccess, SummonFail }
    private AudioSource _audio;
    private IDisolveEffect _dissolve; // SummonMonster 객체에 있는 IDisolveEffect 인터페이스를 할당
    private ISummonType _summonType; // SummonMonster 객체에 있는 ISummonType 인터페이스를 할당

    private void OnEnable()
    {
        _summonMonster.gameObject.SetActive(true);
        _summonType.InitUnitData();
    }

    private void Awake()
    {
        _dissolve = _summonMonster.GetComponent<IDisolveEffect>();
        _audio = GetComponent<AudioSource>();
        _summonType = _summonMonster.GetComponent<ISummonType>();
    }

    public void SetMonsterAnimOn()
    {
        StartCoroutine(_dissolve.DissolveAppear());
    }

    public void SetMonsterAIOn()
    {
        _dissolve.CompleteDissloveAppear();
    }

    public void SetMonsterOff()
    {
        _summonMonster.gameObject.SetActive(false);
    }

    public void PlayAuraSound(int index)
    {
        if (index == -1)
        {
            _audio.Stop();
            return;
        }

        if (index == (int)eSoundClip.SummonLoop && !_audio.isPlaying)
        {
            _audio.clip = _soundClip[index];
            _audio.loop = true;
            _audio.Play();
            return;
        }

        _audio.loop = false;
        _audio.Stop();
        _audio.PlayOneShot(_soundClip[index]);
    }
}
