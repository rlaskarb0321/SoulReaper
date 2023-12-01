using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossAuraAnimCtrl : MonoBehaviour
{
    [SerializeField]
    private float _rotSpeed;

    [SerializeField]
    private ParticleSystem[] _particles;

    private enum eSoundClip { SummonLoop, SummonSuccess, SummonFail }
    private Animator _animator;
    private bool _isReverse;
    private readonly int _hashSuccess = Animator.StringToHash("Success");
    private readonly int _hashFail = Animator.StringToHash("Fail");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
    /// 소환의 성공여부에 관계없이 애니메이션 마지막프레임 이벤트로 해당 오브젝트를 끔
    /// </summary>
    public void EndAnim()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 소환에 성공, 파티클 재생시킴
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
        transform.Rotate(Vector3.up * (reverse * _rotSpeed) * Time.deltaTime);
    }
}
