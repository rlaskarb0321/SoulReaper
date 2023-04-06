using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MonsterBasicStat
{
    public int _health;
    public float _movSpeed;
}

public class Monster : MonoBehaviour
{
    public enum eMonsterLevel { Normal, NormalElite, MiddleBoss, Boss }
    public eMonsterLevel _monsterLevel;
    public MonsterBasicStat _stat;
}
