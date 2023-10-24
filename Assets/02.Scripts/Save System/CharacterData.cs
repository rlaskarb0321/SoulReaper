using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharacterData
{
    public CData _characterData;

    public CharacterData()
    {
        _characterData = new CData("¿€¿∫ Ω£", new Vector3(-116.14f, -4.27f, -65.99f), 100, 100, 50, 50, 5);
    }

    public CharacterData(CData cData)
    {
        _characterData = cData;
    }
}

[Serializable]
public struct CData
{
    public CData(string mapName, Vector3 pos, int currHp, int maxHp, int currMp, int maxMp, int seedCount)
    {
        _mapName = mapName;
        _pos = pos;
        _currHp = currHp;
        _maxHp = maxHp;
        _currMp = currMp;
        _maxMp = maxMp;
        _seedCount = seedCount;
    }

    public string _mapName;
    public Vector3 _pos;
    public int _currHp;
    public int _maxHp;
    public int _currMp;
    public int _maxMp;
    public int _seedCount;
}
