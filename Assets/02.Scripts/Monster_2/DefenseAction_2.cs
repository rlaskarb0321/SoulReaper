using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseAction_2 : MonoBehaviour
{
    private MonsterAI_2 _monsterAI;

    private void Awake()
    {
        _monsterAI = GetComponent<MonsterAI_2>();
    }

    void Start()
    {
        _monsterAI._isDef = true;
    }
}
