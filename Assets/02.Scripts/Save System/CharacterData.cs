using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
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
            float basicAtkDamage                                                            // �÷��̾��� �� �� ���ݷ�
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
    }

    public CData _characterData;

    public CharacterData()
    {
        _characterData = new CData(new Vector3(-116.14f, -4.67f, -65.99f), Quaternion.identity, "LittleForest_Map", 100, 100, 50, 50, 5, 10, 0.0f);
    }

    public CharacterData(CData cData)
    {
        _characterData = cData;
    }
}

