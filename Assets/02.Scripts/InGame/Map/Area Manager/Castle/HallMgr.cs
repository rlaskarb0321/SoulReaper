using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HallMgr : MonoBehaviour
{
    public GameObject[] _unSealTriggers;
    private int _index = 0;

    public void SetUnSealTrigger(int index)
    {
        if (index == -1)
        {
            _unSealTriggers[_index].gameObject.SetActive(false);
            return;
        }

        _unSealTriggers[_index].gameObject.SetActive(true);
        _index = index;
    }
}
