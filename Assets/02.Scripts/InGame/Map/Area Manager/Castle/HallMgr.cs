using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HallMgr : MonoBehaviour
{
    public GameObject[] _unSealTriggers;
    public GameObject _firstMeet;

    public BossGate _bossGate;

    public void SetUnSealTrigger(int index)
    {
        if (index == -1)
        {
            if (_unSealTriggers[0].activeInHierarchy)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }

            _unSealTriggers[index].gameObject.SetActive(false);
            return;
        }

        if (index == 1)
            _bossGate.CanInteract();

        _unSealTriggers[index].gameObject.SetActive(true);
    }

    public void SetOffFirstMeet()
    {
        _bossGate.GetComponent<Animator>().enabled = true;
        _firstMeet.SetActive(false);
    }
}
