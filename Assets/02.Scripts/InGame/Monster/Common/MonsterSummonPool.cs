using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일반 몬스터 소환용 오브젝트 풀
/// </summary>
public class MonsterSummonPool : MonoBehaviour
{
    public GameObject[] _monsterPrefabs;
    public ObjectPooling _projectilePool;

    private int _childCount;

    private void Start()
    {
        _childCount = transform.childCount;
    }

    /// <summary>
    /// 몬스터를 소환할때 호출되는 함수
    /// </summary>
    /// <param name="count"></param>
    public void SummonMonster(int count)
    {
        for (int i = 0; i < _childCount; i++)
        {
            if (count == 0)
                break;
            if (transform.GetChild(i).gameObject.activeSelf)
                continue;

            transform.GetChild(i).gameObject.SetActive(true);
            count--;
        }

        if (count != 0)
        {
            InstantiateRemain(count);
        }
    }

    /// <summary>
    /// 비활성화된 자식 오브젝트로 가지고있던 몬스터보다 많은 양을 소환해야할때 호출하는 함수
    /// </summary>
    /// <param name="count"></param>
    private void InstantiateRemain(int count)
    {
        int childCount = _childCount;
        for (int i = 0; i < childCount; i++)
        {
            if (count == 0)
                return;

            // 몬스터 오브젝트 생성
            int randomValue = Random.Range(0, 2);
            GameObject monster = Instantiate(_monsterPrefabs[randomValue]);

            // 몬스터 오브젝트 위치값 설정
            Transform monsterTr = monster.GetComponent<Transform>();

            monsterTr.gameObject.SetActive(false);
            monsterTr.position = transform.GetChild(i).transform.position;
            monsterTr.parent = this.transform;
            monsterTr.SetAsLastSibling();

            // 몬스터가 원거리라면 발사체 오브젝트 풀링 적용
            LongRange longRange = monster.GetComponentInChildren<LongRange>(true);
            if (longRange != null)
                longRange._projectilePool = _projectilePool;

            // 몬스터 오브젝트 켜주면서 소환
            count--;
            monster.gameObject.SetActive(true);
        }
    }
}
