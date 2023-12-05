using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossAuraAnimCtrl : MonoBehaviour
{
    [SerializeField]
    private float _rotSpeed;

    [SerializeField]
    private GameObject _aura;

    [SerializeField]
    private GameObject[] _particleObj;

    private Animator _animator;
    private ParticleSystem[] _particleComp;
    private bool _isReverse;
    private readonly int _hashSuccess = Animator.StringToHash("Success");
    private readonly int _hashFail = Animator.StringToHash("Fail");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _particleComp = new ParticleSystem[_particleObj.Length];
        for (int i = 0; i < _particleObj.Length; i++)
        {
            _particleComp[i] = _particleObj[i].GetComponent<ParticleSystem>();
        }
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
    /// ��ȯ�� �������ο� ������� �ִϸ��̼� ������������ �̺�Ʈ�� ���� ���·� ��������
    /// </summary>
    public void EndAnim()
    {
        for (int i = 0; i < _particleObj.Length; i++)
        {
            var particleMain = _particleComp[i].main;
            particleMain.loop = true;
        }

        _aura.gameObject.SetActive(false);
    }

    /// <summary>
    /// ��ȯ�� ����
    /// </summary>
    public void SummonSuccess()
    {
        for (int i = 0; i < _particleObj.Length; i++)
        {
            var particleMain = _particleComp[i].main;
            particleMain.loop = false;
        }

        _animator.SetTrigger(_hashSuccess);
    }

    /// <summary>
    /// ��ȯ�� ����
    /// </summary>
    public void SummonFail()
    {
        _animator.SetTrigger(_hashFail);
    }

    /// <summary>
    /// �������� õõ�� ���� ����
    /// </summary>
    private void RotateAura()
    {
        int reverse = _isReverse ? -2 : 1;
        _aura.transform.Rotate(Vector3.up * (reverse * _rotSpeed) * Time.deltaTime);
    }
}
