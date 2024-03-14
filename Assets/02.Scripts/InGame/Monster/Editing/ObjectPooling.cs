using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public List<VFXPool> _pool;
    public Transform _projectileParent;

    // 오브젝트 풀에서 오브젝트를 꺼내는 메서드
    public VFXPool PullOutObject()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
                return _pool[i];
        }

        return AddObject();
    }

    // 오브젝트를 사용해야하는데 풀의 모든 오브젝트가 사용중일때 새로 생성후 풀의 크기를 늘림
    public VFXPool AddObject()
    {
        VFXPool poolObj = Instantiate(_pool[0], _projectileParent);

        poolObj.gameObject.SetActive(false);
        _pool.Add(poolObj);

        return poolObj;
    }
}
