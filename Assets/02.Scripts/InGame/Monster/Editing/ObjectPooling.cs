using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public List<VFXPool> _pool;
    public Transform _projectileParent;

    public VFXPool PullOutObject()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].gameObject.activeSelf)
                return _pool[i];
        }

        return AddObject();
    }

    public VFXPool AddObject()
    {
        VFXPool poolObj = Instantiate(_pool[0], _projectileParent);

        poolObj.gameObject.SetActive(false);
        _pool.Add(poolObj);

        return poolObj;
    }
}
