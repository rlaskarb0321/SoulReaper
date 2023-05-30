using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void PlaySFXs(string sfxName, float volume = 1.0f)
    {
        #region Dict�� ���� ĳ�� case 1
        //int i;
        //if (!_sfxDict.ContainsKey(sfxName))
        //{
        //    for (i = 0; i < _sfxs.Length; i++)
        //    {
        //        if (_sfxs[i].name == sfxName)
        //        {
        //            _sfxDict.Add(_sfxs[i].name, i);

        //            _audio.loop = _sfxs[i].isLoop;
        //            _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //            _audio.PlayOneShot(_sfxs[i].sfx);
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    _sfxDict.TryGetValue(sfxName, out i);

        //    _audio.loop = _sfxs[i].isLoop;
        //    _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
        //    _audio.PlayOneShot(_sfxs[i].sfx);
        //}
        #endregion Dict�� ���� ĳ��

        int i;
        if (_sfxDict.TryGetValue(sfxName, out i))
        {
            _audio.loop = _sfxs[i].isLoop;
            _audio.volume = volume;
            _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
            _audio.PlayOneShot(_sfxs[i].sfx);

            // print("�̹� ����");
        }
        else
        {
            for (i = 0; i < _sfxs.Length; i++)
            {
                if (_sfxs[i].name == sfxName)
                {
                    _sfxDict.Add(_sfxs[i].name, i);

                    _audio.loop = _sfxs[i].isLoop;
                    _audio.volume = volume;
                    _audio.playOnAwake = _sfxs[i].isPlayOnAwake;
                    _audio.PlayOneShot(_sfxs[i].sfx);
                    // print("��� �߰�");
                    break;
                }
            }
        }
    }

    // �ణ�� �����̸� ���� �� ���� ����
    public IEnumerator PlaySFXsDelay(string sfxName, float delay, float volume = 1.0f)
    {
        WaitForSeconds ws = new WaitForSeconds(delay);
        yield return ws;

        PlaySFXs(sfxName, volume);
    }

    // PlayOnAwake�� true�� audioClip ���
    private void PlayOnAwakeSFX()
    {
        for (int i = 0; i < _sfxs.Length; i++)
        {
            if (_sfxs[i].isPlayOnAwake)
            {
                _audio.PlayOneShot(_sfxs[i].sfx);
            }
        }
    }
}
