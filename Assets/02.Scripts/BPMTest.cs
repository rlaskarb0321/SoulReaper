using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPMTest : MonoBehaviour
{
    public float _beat;
    public float _bpm;
    public int _num = 0;

    void Update()
    {
        _beat += Time.deltaTime;
        if (_beat >= _bpm)
        {
            _beat = 0.0f;
            print(++_num + "¹ÚÀÚ");
        }
    }
}
