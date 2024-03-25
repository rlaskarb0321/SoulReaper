using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 환경 설정 값에 대한 정보를 담은 객체
/// </summary>
[Serializable]
public class SettingDataObj
{
    public SData _settingData;

    [Serializable]
    public struct SData
    {
        public float _masterVolume;
        public bool _masterMute;
        public float _bgmVolume;
        public bool _bgmMute;
        public float _sfxVolume;
        public bool _sfxMute;
        public bool _hitShakeState;

        public SData
            (
            float masterVolume = 1.0f,
            bool masterMute = false,
            float bgmVolume = 1.0f,
            bool bgmMute = false,
            float sfxVolume = 1.0f,
            bool sfxMute = false,
            bool hitShakeState = false
            )
        {
            _masterVolume = masterVolume;
            _masterMute = masterMute;
            _bgmVolume = bgmVolume;
            _bgmMute = bgmMute;
            _sfxVolume = sfxVolume;
            _sfxMute = sfxMute;
            _hitShakeState = hitShakeState;
        }
    }

    public SettingDataObj()
    {
        _settingData = new SData(1.0f, false, 1.0f, false, 1.0f, false, false);
    }

    public SettingDataObj(SData sData)
    {
        _settingData = sData;
    }
}
