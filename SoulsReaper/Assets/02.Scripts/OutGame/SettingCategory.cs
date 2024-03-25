using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SettingCategory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("=== �ɼ� ��� ===")]
    public GameObject _categoryContext;

    [Header("=== �� ��ȯ �Ҹ� ===")]
    [SerializeField]
    private AudioClip _changeCategorySound;

    [Header("=== �� ��ȯ ����Ʈ ���� ===")]
    [SerializeField]
    private Color _deactiveColor;

    [SerializeField]
    private Color _activeColor;

    [SerializeField]
    private TMP_Text _categoryText;

    // Field
    private AudioSource _audio;
    private Animator _animator;
    private Image _image;

    // Animator Param
    private readonly int _hashOnPointerEnter = Animator.StringToHash("OnPointerEnter");
    private readonly int _hashSelected = Animator.StringToHash("Selected");

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// �ڱ� ī�װ� �ɼ� ������ ���ְ� �Ҹ� �� ��
    /// </summary>
    public void OpenContext()
    {
        Color color = Color.white;
        color.a = 1.0f;

        _categoryContext.SetActive(true);
        _audio.PlayOneShot(_changeCategorySound, _audio.volume * SettingData._sfxVolume);
        _animator.SetBool(_hashSelected, true);
        _image.color = _activeColor;
        _categoryText.color = color;
    }

    /// <summary>
    /// �ڱ� ī�װ� �ɼ� ������ ��
    /// </summary>
    public void CloseContext()
    {
        Color color = Color.white;
        color.a = 0.1f;

        _categoryContext.SetActive(false);
        _animator.SetBool(_hashSelected, false);
        _image.color = _deactiveColor;
        _categoryText.color = color;
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
