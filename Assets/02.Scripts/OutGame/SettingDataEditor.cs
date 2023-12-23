using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingDataEditor : MonoBehaviour
{
    [Header("=== ī�װ� ===")]
    public SettingCategory _currOpenCategory;

    [Header("=== ���� ===")]
    [Header("=== ������ ���� ===")]
    [SerializeField]
    private Slider _masterVolume;
    [SerializeField]
    private GameObject _masterMuteCheckBox;
    [SerializeField]
    private TMP_InputField _masterValueText;

    [Header("=== ȿ���� ���� ===")]
    [SerializeField]
    private Slider _sfxVolume;
    [SerializeField]
    private GameObject _sfxMuteCheckBox;
    [SerializeField]
    private TMP_InputField _sfxValueText;

    [Header("=== ����� ���� ===")]
    [SerializeField]
    private Slider _bgmVolume;
    [SerializeField]
    private GameObject _bgmMuteCheckBox;
    [SerializeField]
    private TMP_InputField _bgmValueText;

    private void Start()
    {
        ControlAudio();
    }

    /// <summary>
    /// �ɼ� ī�װ��� ������
    /// </summary>
    /// <param name="category"></param>
    public void OpenCategory(SettingCategory category)
    {
        if (_currOpenCategory != null)
            _currOpenCategory.CloseContext();

        _currOpenCategory = category;
        category.OpenContext();
    }

    /// <summary>
    /// üũ�ڽ��� ��Ƽ�� ��븦 ������Ŵ
    /// </summary>
    /// <param name="checkBox"></param>
    public void SwitchCheckBoxState(GameObject checkBox)
    {
        bool activeState = checkBox.activeSelf;

        checkBox.SetActive(!activeState);
        ControlAudio();
    }

    /// <summary>
    /// üũ�ڽ��� ������ ���� ���� ������ �� �� ������ ���̵� �̹����� ����, �ݴ�� ������ �� �� �ֵ��� ����
    /// </summary>
    /// <param name="hideImage"></param>
    public void SwitchHideImage(GameObject hideImage)
    {
        bool activeState = hideImage.activeSelf;
        hideImage.SetActive(!activeState);
    }

    /// <summary>
    /// ���� �ɼ� ��ҵ��� ���� InputField Text�� ������
    /// </summary>
    public void ControlAudio()
    {
        AudioListener.volume = _masterMuteCheckBox.activeSelf == true ? 0.0f : _masterVolume.value;
        _masterValueText.text = ((int)(_masterVolume.value * 100.0f)).ToString();

        SettingData._bgmVolume = _bgmMuteCheckBox.activeSelf == true ? 0.0f : _bgmVolume.value;
        _bgmValueText.text = ((int)(_bgmVolume.value * 100.0f)).ToString();

        SettingData._sfxVolume = _sfxMuteCheckBox.activeSelf == true ? 0.0f : _sfxVolume.value;
        _sfxValueText.text = ((int)(_sfxVolume.value * 100.0f)).ToString();
    }
}
