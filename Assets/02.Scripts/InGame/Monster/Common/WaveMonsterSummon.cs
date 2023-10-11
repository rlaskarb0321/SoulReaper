using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonsterSummon : MonoBehaviour
{
    public MonsterBase_1 _monster;

    public void SetMonsterAnimOn()
    {
        _monster._animator.enabled = true;
        _monster.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void SetMonsterAIOn()
    {
        _monster._nav.enabled = true;
    }
}
