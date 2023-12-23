using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingDataEditor : MonoBehaviour
{
    [Header("=== 카테고리 ===")]
    public SettingCategory _currOpenCategory;

    [Header("=== 사운드 ===")]
    [Header("=== 마스터 볼륨 ===")]
    [SerializeField]
    private Slider _masterVolume;
    [SerializeField]
    private GameObject _masterMuteCheckBox;
    [SerializeField]
    private TMP_InputField _masterValueText;

    [Header("=== 효과음 볼륨 ===")]
    [SerializeField]
    private Slider _sfxVolume;
    [SerializeField]
    private GameObject _sfxMuteCheckBox;
    [SerializeField]
    private TMP_InputField _sfxValueText;

    [Header("=== 배경음 볼륨 ===")]
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
    /// 옵션 카테고리를 열어줌
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
    /// 체크박스의 액티브 상대를 반전시킴
    /// </summary>
    /// <param name="checkBox"></param>
    public void SwitchCheckBoxState(GameObject checkBox)
    {
        bool activeState = checkBox.activeSelf;

        checkBox.SetActive(!activeState);
        ControlAudio();
    }

    /// <summary>
    /// 체크박스가 켜지면 볼륨 조절 영역을 쓸 수 없도록 하이드 이미지를 켜줌, 반대로 꺼지면 쓸 수 있도록 꺼줌
    /// </summary>
    /// <param name="hideImage"></param>
    public void SwitchHideImage(GameObject hideImage)
    {
        bool activeState = hideImage.activeSelf;
        hideImage.SetActive(!activeState);
    }

    /// <summary>
    /// 사운드 옵션 요소들의 값과 InputField Text를 조절함
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
