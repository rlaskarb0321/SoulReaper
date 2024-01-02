using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummonMgr : MonoBehaviour
{
    public MonsterBase _monster;

    private void OnEnable()
    {
        StartCoroutine(ReturnPool());
    }

    private IEnumerator ReturnPool()
    {
        yield return new WaitUntil(() => _monster._state == MonsterBase.eMonsterState.Dead);
        yield return new WaitUntil(() =>  _monster.gameObject.activeSelf == false);

        print("ÀçÈ°¿ë");
        gameObject.SetActive(false);
    }
}
