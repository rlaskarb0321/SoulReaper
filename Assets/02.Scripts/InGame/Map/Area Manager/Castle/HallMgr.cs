using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallMgr : MonoBehaviour
{
    public GameObject[] _unSealTriggers;

    public void SetUnSealTrigger(int index)
    {
        _unSealTriggers[index].gameObject.SetActive(true);
    }
}
