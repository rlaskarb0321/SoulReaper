using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMonsterSummon : MonoBehaviour
{
    public WaveMonster _waveMonster;

    public void SetMonsterAnimOn()
    {
        _waveMonster._monsterBase._animator.enabled = true;
        _waveMonster._monsterBase.GetComponent<CapsuleCollider>().enabled = true;
        StartCoroutine(_waveMonster.DissolveAppear());
    }

    public void SetMonsterAIOn()
    {
        _waveMonster._monsterBase._nav.enabled = true;
        _waveMonster._monsterBase.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
