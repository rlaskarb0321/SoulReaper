using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleForestData : MonoBehaviour
{
    [Header("=== Hierarchy ===")]
    public GameObject _scroll;
    public GameObject _sparrow;
    public HealthPlant _healthFlower;
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

    [Header("=== Forest Map Data ===")]
    [SerializeField]
    private ForestData _mapData;

    private void Awake()
    {
        _mapData = MapDataPackage._mapData._forest._dataStruct;
        ApplyData();
    }

    private void ApplyData()
    {
        if(_mapData._isScrollGet)
        {
            _scroll.SetActive(false);
            _sparrow.SetActive(false);
        }

        print(_healthFlower.FlowerState);
        switch (_healthFlower.FlowerState)
        {
            case HealthPlant.eFlowerState.None:
            case HealthPlant.eFlowerState.Growing:
                break;

            case HealthPlant.eFlowerState.Bloom:
                break;

            case HealthPlant.eFlowerState.harvested:
                break;
        }
    }
}