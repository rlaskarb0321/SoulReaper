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

    public void PopUpSeedUI()
    {
        if (_isContact)
            return;

        _animator.SetBool(_hashContact, true);
    }

    public void GoDownSeedUI()
    {
        _animator.SetBool(_hashContact, false);
    }

    public void EditSeedText(int count)
    {
        _seedCount.text = count.ToString();
    }
}
