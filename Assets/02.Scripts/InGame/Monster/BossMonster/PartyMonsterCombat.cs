using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate bool EditCanUseDelegate();

[Serializable]
public class BossMonsterSkill
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

    /// <summary>
    /// 해당 스킬의 쿨다운 돌리기
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoolDown()
    {
        while (_currCoolTime >= 0.0f)
        {
            _currCoolTime -= Time.deltaTime;
            yield return null;
        }

        _currCoolTime = _coolTime;
    }
}

public class PartyMonsterCombat : MonoBehaviour
{
    public BossMonsterSkill[] _normalStateSkills;
    public BossMonsterSkill[] _tiredStateSkills;
    public bool _isBossTired;

    // Field
    private PartyMonster _monsterBase;
    private GameObject _target;
    private PartyBossPattern _pattern;
    private float _originActDelay;
    private enum ePartyBossSkill 
    {
        // Normal State
        Summon_Mini_Boss,
        Drop_Kick,
        Summon_Normal_Monster,
        Blink,
        Sliding,
        Jump,
        Fist,
        Push,

        // Tired State
        Rest,
        Run,
        Count,
    }

    private void Awake()
    {
        _monsterBase = GetComponent<PartyMonster>();
        _pattern = GetComponent<PartyBossPattern>();
        _target = _monsterBase._target;

        CheckSkill();
    }

    private void Start()
    {
        _originActDelay = _monsterBase._stat.actDelay;
    }

    private void Update()
    {
        if (_monsterBase._state == MonsterBase_1.eMonsterState.Dead)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            CheckSkill();
        }

        print(_monsterBase._state);

        if (!_isBossTired)
        {
            switch (_monsterBase._state)
            {
                // 다음에 행할 스킬을 고르고 실행함
                case MonsterBase_1.eMonsterState.Idle:
                    string skillID = SelectSkill(_normalStateSkills);
                    DoSkill(skillID);
                    break;

                case MonsterBase_1.eMonsterState.Trace:
                    break;

                case MonsterBase_1.eMonsterState.Attack:
                    _monsterBase.AimingTarget(_target.transform.position, 2.0f);
                    break;

                // 공격이 끝난 후 약간의 딜레이 가지기
                case MonsterBase_1.eMonsterState.Delay:
                    if (_monsterBase._stat.actDelay <= 0.0f)
                    {
                        _monsterBase._stat.actDelay = _originActDelay;
                        _monsterBase._state = MonsterBase_1.eMonsterState.Idle;
                        return;
                    }
                     
                    _monsterBase._stat.actDelay -= Time.deltaTime;
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
    private string SelectSkill(BossMonsterSkill[] skillPack)
    {
        string skillID = "";

        // 우선순위는 이미 인스펙터 창에 skillPack 만들 때 정렬해서 만들었음
        for (int i = 0; i < skillPack.Length; i++)
        {
            if (!skillPack[i]._canUse)
            {
                //print(skillPack[i]._id + " 사용 불가능");
                continue;
            }
            if (skillPack[i]._currCoolTime != skillPack[i]._coolTime)
            {
                //print(skillPack[i]._id + " 쿨타임이 안 돌아있음");
                continue;
            }

            //print(skillPack[i]._id + " 로 결정");
            skillID = skillPack[i]._id;
            StartCoroutine(skillPack[i].CoolDown());
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
            #region 일반적인 상태일 때

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

            #endregion 일반적인 상태일 때

            #region 지친 상태일 때
            case ePartyBossSkill.Rest:
                _pattern.Rest();
                break;

            case ePartyBossSkill.Run:
                _pattern.Run();
                break;
            #endregion 지친 상태일 때
        }
    }

    /// <summary>
    /// 스킬을 시전한 후, 애니메이션 마지막에 등록시킬 메서드, 딜레이 상태로 만들어 준다.
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
        (BossMonsterSkill skill, BossMonsterSkill.eSkillUseCondition useCondition, BossMonsterSkill.eSkillUpgrade upgradeCondition)
    {
        switch (useCondition)
        {
            // 페이즈 2때 사용 가능하도록 열어 주기
            case BossMonsterSkill.eSkillUseCondition.Phase2:
                BossMonsterSkill._editCanUseDelegate -= isPhaseTwo;
                BossMonsterSkill._editCanUseDelegate += isPhaseTwo;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;

            case BossMonsterSkill.eSkillUseCondition.Phase3:
                BossMonsterSkill._editCanUseDelegate -= isPhaseThree;
                BossMonsterSkill._editCanUseDelegate += isPhaseThree;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;

            // 멀리있는지 체크 후 사용 가능하도록 열어 주기
            case BossMonsterSkill.eSkillUseCondition.Long:
                BossMonsterSkill._editCanUseDelegate -= isPlayerFar;
                BossMonsterSkill._editCanUseDelegate += isPlayerFar;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;

            // 플레이어가 뒤에있는지 체크 후 사용 가능하도록 열어 주기
            case BossMonsterSkill.eSkillUseCondition.Behind:
                BossMonsterSkill._editCanUseDelegate -= isPlayerBehind;
                BossMonsterSkill._editCanUseDelegate += isPlayerBehind;

                skill._canUse = BossMonsterSkill._editCanUseDelegate();
                break;
        }

        switch (upgradeCondition)
        { 
            case BossMonsterSkill.eSkillUpgrade.Phase2_Up:
                break;
            case BossMonsterSkill.eSkillUpgrade.Phase2_Down:
                break;
        }

    }

    #region 스킬의 CanUSe 를 조작하기 위해 스킬의 Delegate 에 달아놓는 메서드들

    public bool isPlayerBehind()
    {
        // 뒤에있고 가까이에 있는지
        float angle = Vector3.SignedAngle(transform.forward, _target.transform.position - transform.position, transform.up);
        float distance = Vector3.Distance(transform.position, _target.transform.position);

        return Mathf.Abs(angle) >= 100.0f && distance <= 1.0f;
    }

    public bool isPlayerFar()
    {
        return Vector3.Distance(_target.transform.position, transform.position) >= 2.0f;
    }

    public bool isPhaseTwo()
    {
        return _monsterBase.Phase == PartyMonster.ePhase.Phase_2;
    }

    public bool isPhaseThree()
    {
        return _monsterBase.Phase == PartyMonster.ePhase.Phase_3;
    }

    #endregion 스킬의 CanUSe 를 조작하기 위해 스킬의 Delegate 에 달아놓는 메서드들
}