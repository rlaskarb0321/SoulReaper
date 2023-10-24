using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataPackage : MonoBehaviour
{
    public static MapData _mapData; // 로딩씬에서 저장된 파일의 맵 데이터를 이곳에 저장시킴

    private void Awake()
    {
        _mapData = DataManage.LoadMData("TestMData");
    }
}
