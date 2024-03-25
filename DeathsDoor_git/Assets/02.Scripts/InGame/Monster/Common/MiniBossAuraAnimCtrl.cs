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
    /// 소환 마법진의 도는 방향을 바꿔줌
    /// </summary>
    /// <param name="value"></param>
    public void ReverseRotate(int value)
    {
        bool isReverse = value == 1 ? true : false;
        _isReverse = isReverse;
    }

    /// <summary>
    /// 소환의 성공여부에 관계없이 애니메이션 마지막프레임 이벤트로 원래 상태로 돌려놓음
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
    /// 소환에 성공
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
    /// 소환에 실패
    /// </summary>
    public void SummonFail()
    {
        _animator.SetTrigger(_hashFail);
    }

    /// <summary>
    /// 마법진을 천천히 돌게 만듬
    /// </summary>
    private void RotateAura()
    {
        int reverse = _isReverse ? -2 : 1;
        _aura.transform.Rotate(Vector3.up * (reverse * _rotSpeed) * Time.deltaTime);
    }
}
