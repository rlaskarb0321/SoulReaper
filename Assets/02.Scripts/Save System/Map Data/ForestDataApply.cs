using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestDataApply : DataApply, IDataApply
{
    [Header("=== Hierarchy ===")]
    public GameObject _scrollMesh;
    public GameObject _scrollObj;
    public GameObject _sparrow;
    public HealthPlant _healthFlower;
    //public GameObject _shrineSeal;
    //public GameObject _shrineReward;

    // 상부 데이터 관리자에게 제출용 데이터 구조체
    private ForestMap.ForestData _data;

    private void Awake()
    {
        // 게임 데이터 부여
        _data = MapDataPackage._mapData._forest._dataStruct;
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // 스크롤 얻음 여부에따른 오브젝트들 키고 끄기
        if (_data._isScrollGet)
        {
            _scrollMesh.SetActive(false);
            _scrollObj.SetActive(false);
            _sparrow.SetActive(false);
        }

        // 화분에 꽃을 개화시켰는지 수확시켰는지에 따라 오브젝트 상태 바꾸기
        switch (_data._flowerState)
        {
            case HealthPlant.eFlowerState.Bloom:
                _healthFlower.GrownPlant();
                break;
            case HealthPlant.eFlowerState.harvested:
                _healthFlower.HarvestPlant();
                break;
        }
    }

    public override void EditMapData()
    {
        print("Forest Data Apply");

        // 스크롤의 획득 여부를 데이터에 저장
        if (!_scrollMesh.activeSelf)
        {
            _data._isScrollGet = true;
        }

        // 회복 화분의 상태를 데이터에 저장
        _data._flowerState = _healthFlower.FlowerState;

        // 데이터 수정후 Json에 저장하는 작업
        MapDataPackage._mapData._forest._dataStruct = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}