using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossInduce : MonoBehaviour, IYOrNSelectOption
{
    private int _selectNum;

    public void ApplyOption(int selectNum)
    {
        _selectNum = selectNum;
    }

    public void EndDialog()
    {

    }

    public int ReturnSelectResult()
    {
        return _selectNum;
    }
}
