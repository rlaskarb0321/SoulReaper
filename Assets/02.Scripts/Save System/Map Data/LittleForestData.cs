using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleForestData : MonoBehaviour
{
    [Header("=== Hierarchy ===")]
    public GameObject _scroll;
    public GameObject _sparrow;
    //public HealthPlant.eFlowerState _eFlowerState;
    //public GameObject _shrineSeal;
    //public GameObject _shrineReward;

    public ForestData MapData
    {
        get { return _mapData; }
        set
        {
            _mapData = value;
            MapDataPackage._mapData._forest._dataStruct = _mapData;
            DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
        }
    }

    [SerializeField]
    private ForestData _mapData;

    private void Awake()
    {
        _mapData = MapDataPackage._mapData._forest._dataStruct;
    }
}