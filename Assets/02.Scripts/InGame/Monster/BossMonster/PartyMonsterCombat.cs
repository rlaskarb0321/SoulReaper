using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate bool EditCanUseDelegate();

[Serializable]
public class PartyMonsterSkill
{
    // �ش� ��ų�� ����� �� ������ ����
    public bool _canUse;

    // ��ų���� �����ϱ� ���� �̸�
    public string _id;

    // �ش� ��ų�� ���� Ư���� ��Ȳ
    public eSkillUseCondition _eSkillCondition;

    // �ش� ��ų�� ����� ���� ���׷��̵� Ȥ�� �ٿ�׷��̵�Ǵ����� ����
    public eSkillUpgrade _eSkillUpgrade;

    // �ش� ��ų�� �켱����
    public int _priority;

    // �ش� ��ų�� ��Ÿ�� ��
    public float _coolTime;

    // ���� ��ٿ� ���� �ð�
    public float _currCoolTime;

    // ��ų�� ��� ���� ���θ� üũ�ϴ� ��������Ʈ
    public static EditCanUseDelegate _editCanUseDelegate;

    /// <summary>
    /// �ش� ��ų�� ������ �޾��ִ� enum
    /// </summary>
    public enum eSkillUseCondition 
    { 
        None,       // �ش� ��ų�� ��� ���ɿ� ������ ����
        Phase2,     // �ش� ��ų�� phase2 �� ���� ��� ����
        Phase3,     // �ش� ��ų�� phase3 �� ���� ��� ����
        Long,       // �ش� ��ų�� �÷��̾ �ſ� �ָ� ���� �� ��� ��������
        Behind,     // �ش� ��ų�� �÷��̾ �ڽ��� �ڿ� ���� �� ��� ��������
    }

    /// <summary>
    /// �ش� ��ų�� ���׷��̵� ���θ� �˷��ִ� enum
    /// </summary>
    public enum eSkillUpgrade
    { 
        None,           // �ش� ��ų�� ������ ��ȯ �� ���׷��̵� Ȥ�� �ٿ�׷��̵� ���� ����
        Phase2_Up,      // �ش� ��ų�� ������ 2�� ���׷��̵� ��
        Phase2_Down,    // �ش� ��ų�� ������ 2�� �ٿ�׷��̵� ��
        Phase3_Up,      // �ش� ��ų�� ������ 3�� ���׷��̵� ��                                              
        Phase3_Down,    // �ش� ��ų�� ������ 3�� �ٿ�׷��̵� ��
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
    /// ��ų����Ʈ���� ��� �����ϰ� �켱������ ���� ��ų�� ��
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
    /// ����� ��ų�� ID �� �ް�, ������ �����ϰ� �ϴ� �Լ�
    /// </summary>
    /// <param name="skillID"></param>
    private void DoSkill(string skillID)
    {
        //// �� �� �ִ� ��ų�� ���� ����
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
    /// ��ų�� ������ ��, �ִϸ��̼� �������� ��Ͻ�ų �޼���
    /// </summary>
    public void ExitAttackState() => _monsterBase._state = MonsterBase_1.eMonsterState.Delay;

    /// <summary>
    /// ��������Ʈ ȣ���ϴ� �޼��带 �ܺο��� �θ��� �����ϱ� ���� ������ �޼���
    /// </summary>
    public void CheckSkill()
    {
        print("Check Skill");
        for (int i = 0; i < _normalStateSkills.Length; i++)
            EditSkillCondition(_normalStateSkills[i], _normalStateSkills[i]._eSkillCondition, _normalStateSkills[i]._eSkillUpgrade);
    }

    /// <summary>
    /// ��ų���� ��� ���� ���θ� ��Ž���ϰ� �ϴ� ��������Ʈ�� ȣ��
    /// </summary>
    private void EditSkillCondition
        (PartyMonsterSkill skill, PartyMonsterSkill.eSkillUseCondition useCondition, PartyMonsterSkill.eSkillUpgrade upgradeCondition)
    {
        switch (useCondition)
        {
            // ������ 2�� ��� �����ϵ��� ���� �ֱ�
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

            // �ָ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
            case PartyMonsterSkill.eSkillUseCondition.Long:
                PartyMonsterSkill._editCanUseDelegate -= isPlayerFar;
                PartyMonsterSkill._editCanUseDelegate += isPlayerFar;

                skill._canUse = PartyMonsterSkill._editCanUseDelegate();
                break;

            // �÷��̾ �ڿ��ִ��� üũ �� ��� �����ϵ��� ���� �ֱ�
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

    #region ��ų�� CanUSe �� �����ϱ� ���� Delegate �޼����

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

    #endregion ��ų�� CanUSe �� �����ϱ� ���� Delegate �޼����
}