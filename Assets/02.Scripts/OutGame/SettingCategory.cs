using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingCategory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("=== 옵션 목록 ===")]
    public GameObject _categoryContext;

    [Header("=== 탭 전환 소리 ===")]
    [SerializeField]
    private AudioClip _changeCategorySound;

    // Field
    private AudioSource _audio;
    private Animator _animator;

    // Animator Param
    private readonly int _hashOnPointerEnter = Animator.StringToHash("OnPointerEnter");
    private readonly int _hashSelected = Animator.StringToHash("Selected");

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 자기 카테고리 옵션 내용을 켜주고 소리 내 줌
    /// </summary>
    public void OpenContext()
    {
        _categoryContext.SetActive(true);
        _audio.PlayOneShot(_changeCategorySound, _audio.volume * SettingData._sfxVolume);
        _animator.SetBool(_hashSelected, true);
    }

    /// <summary>
    /// 자기 카테고리 옵션 내용을 끔
    /// </summary>
    public void CloseContext()
    {
        _categoryContext.SetActive(false);
        _animator.SetBool(_hashSelected, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animator.SetBool(_hashOnPointerEnter, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetBool(_hashOnPointerEnter, false);
    }
}
