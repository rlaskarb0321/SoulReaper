using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrowSkill : PlayerSkill, IOnOffSwitchSkill
{
    public enum eSkillActiveState { DeActive, Active, Count, }
    public eSkillActiveState _skillActiveState;

    [Header("=== Sound Clip ===")]
    [SerializeField]
    private AudioClip[] _onOffSwitchSound;

    [Header("=== Select Border ===")]
    [SerializeField]
    private GameObject _selectBorder;

    [Header("=== Player Character ===")]
    [SerializeField]
    private PlayerCombat _player;

    // Field
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // ���⿡ ��Ÿ�� ������ �ؾ� ��
    }

    public override void OnMouseDown()
    {

    }

    public override void OnMouseOver()
    {

    }

    public void SwitchActive(bool isAudioPlay)
    {
        // ��ų�� On Off ���¸� �ٲ�
        _skillActiveState++;
        _skillActiveState = (eSkillActiveState)((int)_skillActiveState % (int)eSkillActiveState.Count);
        _selectBorder.SetActive(_skillActiveState == eSkillActiveState.Active);

        if (isAudioPlay)
        {
            _audio.PlayOneShot(_onOffSwitchSound[(int)_skillActiveState]);
        }
        _player.ActiveFireArrow(_skillActiveState, this);
    }

    public override void UseSkill()
    {
        SwitchActive(true);
    }

    public void UseOnOffSkill()
    {
        print("hi");
        SwitchActive(false);
    }
}
