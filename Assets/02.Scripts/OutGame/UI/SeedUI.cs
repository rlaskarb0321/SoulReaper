using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _seedCount;

    private Animator _animator;
    private readonly int _hashContact = Animator.StringToHash("isContact");
    private bool _isContact;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isContact = _animator.GetBool(_hashContact);
    }

    /// <summary>
    /// 씨앗에 가까이갔을 때, 관련 UI를 애니메이션으로 띄우기
    /// </summary>
    public void PopUpSeedUI()
    {
        if (_isContact)
            return;

        _animator.SetBool(_hashContact, true);
    }

    /// <summary>
    /// 씨앗에 가까이간 후, 멀어졌을 때 관련 UI를 애니메이션으로 감추기
    /// </summary>
    public void GoDownSeedUI()
    {
        _animator.SetBool(_hashContact, false);
    }

    /// <summary>
    /// 씨앗의 보유 개수를 수정
    /// </summary>
    /// <param name="count"></param>
    public void EditSeedText(int count)
    {
        _seedCount.text = $"X {count.ToString()}";
    }
}
