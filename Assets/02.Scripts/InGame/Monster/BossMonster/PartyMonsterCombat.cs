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
    [HideInInspector]
    public eSkillUpgrade _eSkillUpgrade;

    // �ش� ��ų�� �켱����
    [HideInInspector]
    public int _priority;

    // �ش� ��ų�� ��Ÿ�� ��
    [HideInInspector]
    public float _coolTime;

    // ���� ��ٿ� ���� �ð�
    [HideInInspector]
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
            // ������ 2�� ��� �����ϵ��� ���� �ֱ�
            case PartyMonsterSkill.eSkillUseCondition.Phase2:
                PartyMonsterSkill._editCanUseDelegate -= isPhaseTwo;
                PartyMonsterSkill._editCanUseDelegate += isPhaseTwo;

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