using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ĳ���Ϳ� ���� ������ ����ִ� ��ü
/// </summary>
[Serializable]
public class CharacterData
{
    /// <summary>
    /// ĳ���Ϳ� ���� ������ �����ϴ� ����ü
    /// </summary>
    [Serializable]
    public struct CData
    {
        public CData
            (
            Vector3 pos,                                                                    // �÷��̾��� ��ġ
            Quaternion rot,                                                                 // �÷��̾��� ȸ����
            string mapName,                                                                 // �÷��̾��� ������ ������ �� �ִ� ���̸�
            float currHp,                                                                   // ���� ���� hp
            float maxHp,                                                                    // �ִ� hp
            float currMp,                                                                   // ���� ���� mp
            float maxMp,                                                                    // �ִ� mp
            int seedCount,                                                                  // ������ � �����ִ���
            int soulCount,                                                                  // �÷��̾ ������ �ִ� ��ȥ
            float basicAtkDamage,                                                           // �÷��̾��� �� �� ���ݷ�
            string[] skillList                                                              // ��ų ����Ʈ
            )
        {
            _pos = pos;
            _rot = rot;
            _mapName = mapName;
            _currHP = currHp;
            _maxHP = maxHp;
            _currMP = currMp;
            _maxMP = maxMp;
            _seedCount = seedCount;
            _soulCount = soulCount;
            _basicAtkDamage = basicAtkDamage;
            _skillArray = skillList;
        }

        public Vector3 _pos;
        public Quaternion _rot;
        public string _mapName;
        public float _currHP;
        public float _maxHP;
        public float _currMP;
        public float _maxMP;
        public int _seedCount;
        public int _soulCount;
        public float _basicAtkDamage;
        public string[] _skillArray;
    }

    /// <summary>
    /// �Ű����� ���� ����Ǹ� �⺻������ CData �ʱ�ȭ
    /// </summary>
    public CharacterData()
    {
        _characterData = new CData(new Vector3(-116.14f, -4.67f, -65.99f), Quaternion.identity, "LittleForest_Map", 100, 100, 50, 50, 5, 10, 0.0f, new string[ConstData.SKILL_UI_COUNT]);
    }

    /// <summary>
    /// �Ű����� ������ �ִ°����� CData �ʱ�ȭ
    /// </summary>
    /// <param name="cData"></param>
    public CharacterData(CData cData)
    {
        _characterData = cData;
    }

    public CData _characterData;
}

[Serializable]
public class BuffData
{
    [Serializable]
    public struct BData
    {
        public BData(PlayerBuff buff, int duration)
        {
            _buff = buff;
            _duration = duration;
        }

        public PlayerBuff _buff;
        public int _duration;
    }

    public BuffData()
    {
        _dataList = new List<BData>();
    }

    public BuffData(List<BData> dataList)
    {
        _dataList = dataList;
    }

    public List<BData> _dataList;
}