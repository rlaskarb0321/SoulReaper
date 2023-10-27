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
            float currHp,                                                                     // ���� ���� hp
            float maxHp,                                                                      // �ִ� hp
            float currMp,                                                                     // ���� ���� mp
            float maxMp,                                                                      // �ִ� mp
            int seedCount                                                                   // ������ � �����ִ���
            )
        {
            _pos = pos;
            _currHP = currHp;
            _maxHP = maxHp;
            _currMP = currMp;
            _maxMP = maxMp;
            _seedCount = seedCount;
        }

        public Vector3 _pos;
        public float _currHP;
        public float _maxHP;
        public float _currMP;
        public float _maxMP;
        public int _seedCount;
    }

    public CData _characterData;

    public CharacterData()
    {
        _characterData = new CData(new Vector3(-116.14f, -4.67f, -65.99f), 100, 100, 50, 50, 5);
    }

    public CharacterData(CData cData)
    {
        _characterData = cData;
    }
}

