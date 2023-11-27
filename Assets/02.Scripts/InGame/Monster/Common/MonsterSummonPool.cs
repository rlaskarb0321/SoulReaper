using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSummonPool : MonoBehaviour
{
    [SerializeField]
    private GameObject _effectParent;

    [SerializeField]
    private MonsterSummon[] _summonAura;

    private void Awake()
    {
        _summonAura = _effectParent.GetComponentsInChildren<MonsterSummon>(true);
    }

    public void SummonMonster(int count)
    {
        for (int i = 0; i < _summonAura.Length; i++)
        {
            if (count == 0)
                break;
            if (_summonAura[i].gameObject.activeSelf)
                continue;

            _summonAura[i].gameObject.SetActive(false);
            _summonAura[i].gameObject.SetActive(true);
            count--;
        }

        if (count != 0)
        {
            print(count + " 남은만큼 instantiate");
        }
    }
}
