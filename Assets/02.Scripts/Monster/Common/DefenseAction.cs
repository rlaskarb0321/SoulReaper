using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseAction : MonoBehaviour
{
    private MonsterAI _monsterAI;

    private void Awake()
    {
        _monsterAI = GetComponent<MonsterAI>();
    }

    void Start()
    {
        _monsterAI._isDef = true;
    }
}
