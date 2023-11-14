using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate bool EditCanUseDelegate();

[Serializable]
public class PartyMonsterSkill
{
    // 해당 스킬을 사용할 수 있음의 여부
    public bool _canUse;

    // 스킬끼리 구분하기 위한 이름
    public string _id;

    // 해당 스킬이 사용될 특수한 상황
    public eSkillUseCondition _eSkillCondition;

    // 해당 스킬이 페이즈에 따라 업그레이드 혹은 다운그레이드되는지의 여부
    [HideInInspector]
    public eSkillUpgrade _eSkillUpgrade;

    // 해당 스킬의 우선순위
    [HideInInspector]
    public int _priority;

    // 해당 스킬의 쿨타임 값
    [HideInInspector]
    public float _coolTime;

    // 현재 쿨다운 중인 시간
    [HideInInspector]
    public float _currCoolTime;

    // 스킬의 사용 가능 여부를 체크하는 델리게이트
    public static EditCanUseDelegate _editCanUseDelegate;

    /// <summary>
    /// 해당 스킬에 조건을 달아주는 enum
    /// </summary>
    public enum eSkillUseCondition 
    { 
        None,       // 해당 스킬은 사용 가능에 조건이 없음
        Phase2,     // 해당 스킬은 phase2 때 부터 사용 가능
        Long,       // 해당 스킬은 플레이어가 매우 멀리 있을 때 사용 가능해짐
        Behind,     // 해당 스킬은 플레이어가 자신의 뒤에 있을 때 사용 가능해짐
    }

    /// <summary>
    /// 해당 스킬의 업그레이드 여부를 알려주는 enum
    /// </summary>
    public enum eSkillUpgrade
    { 
        None,           // 해당 스킬은 페이즈 변환 때 업그레이드 혹은 다운그레이드 되지 않음
        Phase2_Up,      // 해당 스킬은 페이즈 2때 업그레이드 됨
        Phase2_Down,    // 해당 스킬은 페이즈 2때 다운그레이드 됨
    }
}

public class PartyMonsterCombat : MonoBehaviour
{
    public PartyMonsterSkill[] _normalStateSkills;
    public PartyMonsterSkill[] _tiredStateSkills;
    public bool _isBossTired;

    // Field
    private MonsterBase_1 _monsterBase;
    private GameObject _target;

    private void Awake()
    {
        _monsterBase = GetComponent<MonsterBase_1>();
        _target = _monsterBase._target;

        CheckSkill();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckSkill();
        }
    }

    private void CheckSkill()
    {
        for (int i = 0; i < _normalStateSkills.Length; i++)
            EditSkillCondition(_normalStateSkills[i], _normalStateSkills[i]._eSkillCondition, _normalStateSkills[i]._eSkillUpgrade);
    }

    private void EditSkillCondition
        (PartyMonsterSkill skill, PartyMonsterSkill.eSkillUseCondition useCondition, PartyMonsterSkill.eSkillUpgrade upgradeCondition)
    {
        switch (useCondition)
        {
            // 페이즈 2때 사용 가능하도록 열어 주기
            case PartyMonsterSkill.eSkillUseCondition.Phase2:
                PartyMonsterSkill._editCanUseDelegate -= isPhaseTwo;
                PartyMonsterSkill._editCanUseDelegate += isPhaseTwo;

                skill._canUse = PartyMonsterSkill._editCanUseDelegate();
                break;

            // 멀리있는지 체크 후 사용 가능하도록 열어 주기
            case PartyMonsterSkill.eSkillUseCondition.Long:
                PartyMonsterSkill._editCanUseDelegate -= isPlayerFar;
                PartyMonsterSkill._editCanUseDelegate += isPlayerFar;

                skill._canUse = PartyMonsterSkill._editCanUseDelegate();
                break;

            // 플레이어가 뒤에있는지 체크 후 사용 가능하도록 열어 주기
            case PartyMonsterSkill.eSkillUseCondition.Behind:
                PartyMonsterSkill._editCanUseDelegate -= isPlayerBehind;
                PartyMonsterSkill._editCanUseDelegate += isPlayerBehind;

                skill._canUse = PartyMonsterSkill._editCanUseDelegate();
                break;
        }

        switch (upgradeCondition)
        { 
            case PartyMonsterSkill.eSkillUpgrade.Phase2_Up:
                break;
            case PartyMonsterSkill.eSkillUpgrade.Phase2_Down:
                break;
        }

    }

    public bool isPlayerBehind()
    {
        return false;
    }

    public bool isPlayerFar()
    {
        return Vector3.Distance(_target.transform.position, transform.position) >= 10.0f;
    }

    public bool isPhaseTwo()
    {
        return true;
    }
}