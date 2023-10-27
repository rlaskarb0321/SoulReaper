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

    // ��� ������ �����ڿ��� ����� ������ ����ü
    private ForestMap.ForestData _data;

    private void Awake()
    {
        // ���� ������ �ο�
        _data = MapDataPackage._mapData._forest._dataStruct;
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // ��ũ�� ���� ���ο����� ������Ʈ�� Ű�� ����
        if (_data._isScrollGet)
        {
            _scrollMesh.SetActive(false);
            _scrollObj.SetActive(false);
            _sparrow.SetActive(false);
        }

        // ȭ�п� ���� ��ȭ���״��� ��Ȯ���״����� ���� ������Ʈ ���� �ٲٱ�
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

        // ��ũ���� ȹ�� ���θ� �����Ϳ� ����
        if (!_scrollMesh.activeSelf)
        {
            _data._isScrollGet = true;
        }

        // ȸ�� ȭ���� ���¸� �����Ϳ� ����
        _data._flowerState = _healthFlower.FlowerState;

        // ������ ������ Json�� �����ϴ� �۾�
        MapDataPackage._mapData._forest._dataStruct = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}