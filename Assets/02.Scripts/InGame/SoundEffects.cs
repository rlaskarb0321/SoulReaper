using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������Ʈ�� �� �� �ִ� audioClip, �� �� ������ �����ִ� ����ü
[System.Serializable]
public struct SFXs
{
    public string name;
    public AudioClip sfx;
    public bool isLoop;
    public bool isPlayOnAwake;
}

// bgm�� ui ���� ȿ������ ������, �� ���ӳ� ������Ʈ���� ȿ���� ���� ��ũ��Ʈ

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

    // Ÿ ��ũ��Ʈ���� �� ��ũ��Ʈ�� ���� ���ӿ�����Ʈ �Ҹ��� �����Ű�� ���� �Լ�
    public void PlayOneShotUsingDict(string sfxName, float volume = 1.0f)
    {
        if (volume == 1.0f)
            volume = _audio.volume;

        int i = ReturnSFXIndex(sfxName);
        if (i == -1)
        {
            print("sfxName ���� sfxs �� �����ϴ�");
            return;
        }

        _audio.loop = _sfxs[i].isLoop;
        _audio.volume = volume;
        _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        _audio.PlayOneShot(_sfxs[i].sfx, _audio.volume * SettingData._sfxVolume);

        #region 23.10.17 Dict �۾� �޼���� �и�
        //int i;
        //// Dict �� sfxName ���� �� Ű���� �������
        //if (_sfxDict.TryGetValue(sfxName, out i))
        //{
        //    _audio.loop = _sfxs[i].isLoop;
        //    _audio.volume = volume;
        //    _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //    _audio.PlayOneShot(_sfxs[i].sfx);
        //}
        //// �������
        //else
        //{
        //    // �Էµ� sfxName����, �ش� ��ü�� �÷����� �� �ִ� clip ������ Ȯ���� Dict�� �߰�
        //    for (i = 0; i < _sfxs.Length; i++)
        //    {
        //        // ��ġ���� Ȯ��
        //        if (_sfxs[i].name == sfxName)
        //        {
        //            _sfxDict.Add(_sfxs[i].name, i);

        //            _audio.loop = _sfxs[i].isLoop;
        //            _audio.volume = volume;
        //            _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //            _audio.PlayOneShot(_sfxs[i].sfx);
        //            break;
        //        }
        //    }
        //}
        #endregion 23.10.17 Dict �۾� �޼���� �и�
    }

    public void PlayUsingDict(string sfxName, float volume = 1.0f)
    {
        if (volume == 1.0f)
            volume = _audio.volume;

        int i = ReturnSFXIndex(sfxName);
        if (i == -1)
        {
            print("sfxName ���� sfxs �� �����ϴ�");
            return;
        }

        _audio.loop = _sfxs[i].isLoop;
        _audio.volume = volume;
        _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        _audio.clip = _sfxs[i].sfx;
        _audio.Play();
    }

    public void Stop() => _audio.Stop();

    public bool IsPlaying()
    {
        return _audio.isPlaying;
    }

    public int ReturnSFXIndex(string sfxName)
    {
        int i;

        if(_sfxDict.TryGetValue(sfxName, out i))
        {
            return i;
        }
        else
        {
            for (i = 0; i < _sfxs.Length; i++)
            {
                if (_sfxs[i].name == sfxName)
                {
                    _sfxDict.Add(_sfxs[i].name, i);
                    return i;
                }
            }

            return -1;
        }
    }

    // �ణ�� �����̸� ���� �� ���� ����
    public IEnumerator PlaySFXsDelay(string sfxName, float delay, float volume = 1.0f)
    {
        WaitForSeconds ws = new WaitForSeconds(delay);
        yield return ws;

        PlayOneShotUsingDict(sfxName, volume);
    }

    // PlayOnAwake�� true�� audioClip ���
    private void PlayOnAwakeSFX()
    {
        for (int i = 0; i < _sfxs.Length; i++)
        {
            if (_sfxs[i].isPlayOnAwake)
            {
                _audio.PlayOneShot(_sfxs[i].sfx, _audio.volume * SettingData._sfxVolume);
            }
        }
    }
}
