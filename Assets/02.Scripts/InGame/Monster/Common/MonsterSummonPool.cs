using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Ϲ� ���� ��ȯ�� ������Ʈ Ǯ
/// </summary>
public class MonsterSummonPool : MonoBehaviour
{
    [Header("=== ���̷�Ű ===")]
    [SerializeField]
    private GameObject _effectParent;

    [SerializeField]
    private GameObject _monsterParent;

    [SerializeField]
    private ObjectPooling _projectilePool;

    [Header("=== ������� ������ ===")]
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

            // ������ ������Ʈ ������ �ʱ�ȭ
            summonAura.gameObject.SetActive(false);
            summonAura._summonMonster = monster.gameObject;
            summonAura.transform.position = _monsterPool[i].transform.position;
            summonAura.transform.parent = _effectParent.transform;
            summonAura.transform.SetAsLastSibling();

            // ���� Ÿ�� ���� �� �ʱ�ȭ
            monster._aura = summonAura.gameObject;
            monster.transform.position = summonAura.transform.position;
            monster.transform.parent = _monsterParent.transform;
            monster.transform.SetAsLastSibling();

            // ����ü Ǯ ����
            LongRange longMonster = monster.GetComponent<LongRange>();
            if (longMonster != null)
                longMonster._projectilePool = _projectilePool;

            // ���� Ǯ�� �߰�
            _monsterPool.Add(summonAura);
            count--;

            // ��� �ʱ�ȭ�� ������ Ȱ��ȭ
            summonAura.gameObject.SetActive(true);
        }
    }
}
