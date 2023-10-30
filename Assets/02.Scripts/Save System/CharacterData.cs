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
            Vector3 pos,                                                                    // 플레이어의 위치
            Quaternion rot,                                                                 // 플레이어의 회전값
            string mapName,                                                                 // 플레이어의 마지막 저장할 때 있던 맵이름
            float currHp,                                                                   // 현재 남은 hp
            float maxHp,                                                                    // 최대 hp
            float currMp,                                                                   // 현재 남은 mp
            float maxMp,                                                                    // 최대 mp
            int seedCount,                                                                  // 씨앗을 몇개 갖고있는지
            int soulCount                                                                   // 플레이어가 가지고 있는 영혼
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
    }

    public CData _characterData;

    public CharacterData()
    {
        _characterData = new CData(new Vector3(-116.14f, -4.67f, -65.99f), Quaternion.identity, "LittleForest_Map", 100, 100, 50, 50, 5, 10);
    }

    public CharacterData(CData cData)
    {
        _characterData = cData;
    }
}

