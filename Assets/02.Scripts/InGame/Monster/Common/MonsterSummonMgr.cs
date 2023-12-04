using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummonMgr : MonoBehaviour
{
    public MonsterBase_1 _monster;

    private void OnEnable()
    {
        StartCoroutine(ReturnPool());
    }

    private IEnumerator ReturnPool()
    {
        yield return new WaitUntil(() => _monster._state == MonsterBase_1.eMonsterState.Dead);
        yield return new WaitUntil(() =>  _monster.gameObject.activeSelf == false);

        print("ÀçÈ°¿ë");
        gameObject.SetActive(false);
    }
}
