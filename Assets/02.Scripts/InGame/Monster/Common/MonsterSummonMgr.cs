using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummonMgr : MonoBehaviour
{
    public MonsterBase_1 _monster;
    public GameObject _aura;

    private Animator _auraAnim;

    private void Awake()
    {
        _auraAnim = _aura.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _monster.gameObject.SetActive(false);
        _auraAnim.enabled = false;
        _auraAnim.enabled = true;
        _aura.gameObject.SetActive(true);

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
