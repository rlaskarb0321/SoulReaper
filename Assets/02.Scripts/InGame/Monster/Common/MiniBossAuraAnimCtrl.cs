using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossAuraAnimCtrl : MonoBehaviour
{
    [SerializeField]
    private float _rotSpeed;

    [SerializeField]
    private ParticleSystem[] _particles;

    [SerializeField]
    private AudioClip[] _soundClip;

    private enum eSoundClip { SummonLoop, SummonSuccess, SummonFail }
    private AudioSource _audio;
    private Animator _animator;
    private bool _isReverse;
    private readonly int _hashSuccess = Animator.StringToHash("Success");
    private readonly int _hashFail = Animator.StringToHash("Fail");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        RotateAura();
    }

    /// <summary>
    /// ��ȯ �������� ���� ������ �ٲ���
    /// </summary>
    /// <param name="value"></param>
    public void ReverseRotate(int value)
    {
        bool isReverse = value == 1 ? true : false;
        _isReverse = isReverse;
    }

    /// <summary>
    /// ��ȯ�� �������ο� ������� �ִϸ��̼� ������������ �̺�Ʈ�� �ش� ������Ʈ�� ��
    /// </summary>
    public void EndAnim()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ��ȯ�� ����, ��ƼŬ �����Ŵ
    /// </summary>
    public void SummonSuccess()
    {
        _animator.SetTrigger(_hashSuccess);
        for (int i = 0; i < _particles.Length; i++)
        {
            _particles[i].Play();
        }
    }

    /// <summary>
    /// ��ȯ�� ����
    /// </summary>
    public void SummonFail()
    {
        _animator.SetTrigger(_hashFail);
    }

    /// <summary>
    /// �ִϸ��̼� �̺�Ʈ�� �Ҹ��� ���Ե� Ÿ�̹��� ����, ���ܷ� ��ȯ�����϶� ����� Ŭ���� ������ ������ ���ְ� �ְ� �÷���
    /// </summary>
    /// <param name="index"></param>
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

    /// <summary>
    /// �������� õõ�� ���� ����
    /// </summary>
    private void RotateAura()
    {
        int reverse = _isReverse ? -2 : 1;
        transform.Rotate(Vector3.up * (reverse * _rotSpeed) * Time.deltaTime);
    }
}
