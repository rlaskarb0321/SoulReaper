using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일반 몬스터 소환용 오브젝트 풀
/// </summary>
public class MonsterSummonPool : MonoBehaviour
{
    [Header("=== 하이러키 ===")]
    [SerializeField]
    private GameObject _effectParent;

    [SerializeField]
    private GameObject _monsterParent;

    [SerializeField]
    private ObjectPooling _projectilePool;

    [Header("=== 재생성용 프리팹 ===")]
    [SerializeField]
    private GameObject _auraPrefab;

    [SerializeField]
    private GameObject[] _monsterPrefabs;

    // Field
    private List<MonsterSummon> _monsterPool;

    private void Awake()
    {
        _monsterPool = new List<MonsterSummon>();
        for (int i = 0; i < _effectParent.transform.childCount; i++)
        {
            _monsterPool.Add(_effectParent.transform.GetChild(i).GetComponent<MonsterSummon>());
        }
    }

    public void SummonMonster(int count)
    {
        for (int i = 0; i < _monsterPool.Count; i++)
        {
            if (count == 0)
                break;
            if (_monsterPool[i].gameObject.activeSelf)
                continue;

            _monsterPool[i].gameObject.SetActive(false);
            _monsterPool[i].gameObject.SetActive(true);
            count--;
        }

        if (count != 0)
        {
            InstantiateRemain(count);
        }
    }

    private void InstantiateRemain(int count)
    {
        for (int i = 0; i < _monsterPool.Count; i++)
        {
            if (count == 0)
                break;

            int randomValue = Random.Range(0, 2);

            MonsterSummon summonAura = Instantiate(_auraPrefab).GetComponent<MonsterSummon>();
            NormalSummonType monster = Instantiate(_monsterPrefabs[randomValue]).GetComponent<NormalSummonType>();

            // 오오라 오브젝트 생성과 초기화
            summonAura.gameObject.SetActive(false);
            summonAura._summonMonster = monster.gameObject;
            summonAura.transform.position = _monsterPool[i].transform.position;
            summonAura.transform.parent = _effectParent.transform;
            summonAura.transform.SetAsLastSibling();

            // 몬스터 타입 선택 및 초기화
            monster._aura = summonAura.gameObject;
            monster.transform.position = summonAura.transform.position;
            monster.transform.parent = _monsterParent.transform;
            monster.transform.SetAsLastSibling();

            // 투사체 풀 적용
            LongRange longMonster = monster.GetComponent<LongRange>();
            if (longMonster != null)
                longMonster._projectilePool = _projectilePool;

            // 몬스터 풀에 추가
            _monsterPool.Add(summonAura);
            count--;

            // 모든 초기화가 끝나고 활성화
            summonAura.gameObject.SetActive(true);
        }
    }
}
