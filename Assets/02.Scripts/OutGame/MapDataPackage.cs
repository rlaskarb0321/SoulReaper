using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataPackage : MonoBehaviour
{
    public static MapData _mapData; // �ε������� ����� ������ �� �����͸� �̰��� �����Ŵ

    private void Awake()
    {
        _mapData = DataManage.LoadMData("TestMData");
    }
}
