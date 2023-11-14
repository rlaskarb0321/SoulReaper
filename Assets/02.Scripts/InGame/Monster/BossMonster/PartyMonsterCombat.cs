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
    public eSkillUpgrade _eSkillUpgrade;

    // 해당 스킬의 우선순위
    public int _priority;

    // 해당 스킬의 쿨타임 값
    public float _coolTime;

    // 현재 쿨다운 중인 시간
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
        Phase3,     // 해당 스킬은 phase3 때 부터 사용 가능
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
        Phase3_Up,      // 해당 스킬은 페이즈 3때 업그레이드 됨                                              
        Phase3_Down,    // 해당 스킬은 페이즈 3때 다운그레이드 됨
    }
}

public class PartyMonsterCombat : MonoBehaviour
{
    public PartyMonsterSkill[] _normalStateSkills;
    public PartyMonsterSkill[] _tiredStateSkills;
    public bool _isBossTired;

    // Field
    private PartyMonster _monsterBase;
    private GameObject _target;
    private PartyBossPattern _pattern;
    private enum ePartyBossSkill 
    {
        Summon_Mini_Boss,
        Drop_Kick,
        Summon_Normal_Monster,
        Blink,
        Sliding,
        Jump,
        Fist,
        Push,
        Count,
    }

    private void Awake()
    {
        _monsterBase = GetComponent<PartyMonster>();
        _pattern = GetComponent<PartyBossPattern>();
        _target = _monsterBase._target;

        CheckSkill();
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckSkill();
        }

        if (!_isBossTired)
        {
            switch (_monsterBase._state)
            {
                case MonsterBase_1.eMonsterState.Idle:
                    string skillID = SelectSkill(_normalStateSkills);
                    DoSkill(skillID);
                    break;

                case MonsterBase_1.eMonsterState.Trace:
                    break;

                case MonsterBase_1.eMonsterState.Attack:
                    _monsterBase.AimingTarget(_target.transform.position, 2.0f);
                    break;

                case MonsterBase_1.eMonsterState.Delay:
                    break;
            }
        }
        else
        {
            switch (_monsterBase._state)
            {
                case MonsterBase_1.eMonsterState.Idle:
                    string skillID = SelectSkill(_tiredStateSkills);
                    print(skillID);
                    break;

                case MonsterBase_1.eMonsterState.Trace:
                    break;

                case MonsterBase_1.eMonsterState.Attack:
                    break;

                case MonsterBase_1.eMonsterState.Delay:
                    break;
            }
        }

    }

    /// <summary>
    /// 스킬리스트에서 사용 가능하고 우선순위가 높은 스킬을 고름
    /// </summary>
    private string SelectSkill(PartyMonsterSkill[] skillPack)
    {
        string skillID = "";
        for (int i = 0; i < skillPack.Length; i++)
        {
            if (!skillPack[i]._canUse)
                continue;
            if (skillPack[i]._currCoolTime != skillPack[i]._coolTime)
                continue;

            skillID = skillPack[i]._id;
            return skillID;
        }

        return skillID;
    }

    /// <summary>
    /// 사용할 스킬의 ID 를 받고, 실제로 실행하게 하는 함수
    /// </summary>
    /// <param name="skillID"></param>
    private void DoSkill(string skillID)
    {
        //// 쓸 수 있는 스킬이 없는 상태
        //if (skillID == null || skillID == "")
        //{
        //}

        int skillIndex = int.Parse(skillID.Split('_')[0]);
        ePartyBossSkill bossSkill = (ePartyBossSkill)skillIndex;
        _monsterBase._state = MonsterBase_1.eMonsterState.Attack;
        switch (bossSkill)
        {
            case ePartyBossSkill.Summon_Mini_Boss:
                _pattern.SummonMiniBoss();
                break;

            case ePartyBossSkill.Drop_Kick:
                _pattern.DropKick();
                break;

            case ePartyBossSkill.Summon_Normal_Monster:
                _pattern.SummonNormalMonster();
                break;

            case ePartyBossSkill.Blink:
                _pattern.Blink();
                break;

            case ePartyBossSkill.Sliding:
                _pattern.Sliding();
                break;

            case ePartyBossSkill.Jump:
                _pattern.Jump();
                break;

            case ePartyBossSkill.Fist:
                _pattern.Fist();
                break;

            case ePartyBossSkill.Push:
                _pattern.Push();
                break;
        }
    }

    /// <summary>
    /// 스킬을 시전한 후, 애니메이션 마지막에 등록시킬 메서드
    /// </summary>
    public void ExitAttackState() => _monsterBase._state = MonsterBase_1.eMonsterState.Delay;

    /// <summary>
    /// 델리게이트 호출하는 메서드를 외부에서 부르기 쉽게하기 위해 선언한 메서드
    /// </summary>
    public void CheckSkill()
    {
        print("Check Skill");
        for (int i = 0; i < _normalStateSkills.Length; i++)
            EditSkillCondition(_normalStateSkills[i], _normalStateSkills[i]._eSkillCondition, _normalStateSkills[i]._eSkillUpgrade);
    }

    /// <summary>
    /// 스킬들의 사용 가능 여부를 재탐색하게 하는 델리게이트를 호출
    /// </summary>
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

            case PartyMonsterSkill.eSkillUseCondition.Phase3:
                PartyMonsterSkill._editCanUseDelegate -= isPhaseThree;
                PartyMonsterSkill._editCanUseDelegate += isPhaseThree;

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

    #region 스킬의 CanUSe 를 조작하기 위한 Delegate 메서드들

    public bool isPlayerBehind()
    {
        float angle = Vector3.SignedAngle(transform.forward, _target.transform.position - transform.position, transform.up);
        return Mathf.Abs(angle) >= 100.0f;
    }

    public bool isPlayerFar()
    {
        return Vector3.Distance(_target.transform.position, transform.position) >= 10.0f;
    }

    public bool isPhaseTwo()
    {
        return _monsterBase.Phase == PartyMonster.ePhase.Phase_2;
    }

    public bool isPhaseThree()
    {
        return _monsterBase.Phase == PartyMonster.ePhase.Phase_3;
    }

    #endregion 스킬의 CanUSe 를 조작하기 위한 Delegate 메서드들
}