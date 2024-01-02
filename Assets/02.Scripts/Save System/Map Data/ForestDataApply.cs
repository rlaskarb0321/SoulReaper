using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestDataApply : DataApply, IDataApply
{
    [Header("=== ���̾Ű ===")]
    public GameObject _scrollMesh;
    public GameObject _scrollObj;
    public GameObject _sparrow;
    public HealthPlant _healthFlower;
    public MonsterBase _gateKeeper;
    public BoxCollider _castleGate;
    public GameObject _castleBattleICon;
    public BullInteract _bullInteract;

    [Header("=== ��Ȱ ����Ʈ ===")]
    public ReviveStone _reviveStone;

    // ��� ������ �����ڿ��� ����� ������ ����ü
    private ForestMap.ForestData _data;

    private void Awake()
    {
        // �÷��̾� ĳ���� ������� �� ��Ȱ
        _reviveStone.Revive();

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

        if (_data._isGateKeeperDead)
        {
            _gateKeeper.gameObject.SetActive(false);
            _castleGate.enabled = true;
            _castleBattleICon.SetActive(false);
        }

        if (_data._isGateOpen)
        {
            _bullInteract._isGateOpen = true;
            _castleGate.enabled = true;
            _castleBattleICon.SetActive(false);
        }
    }

    public override void EditData()
    {
        print("Forest Data Apply");

        // ��ũ���� ȹ�� ���θ� �����Ϳ� ����
        if (!_scrollMesh.activeSelf)
        {
            _data._isScrollGet = true;
        }

        // ȸ�� ȭ���� ���¸� �����Ϳ� ����
        _data._flowerState = _healthFlower.FlowerState;

        // �������� ��� ���θ� �����Ϳ� ����
        _data._isGateKeeperDead = _gateKeeper._state == MonsterBase.eMonsterState.Dead ? true : false;

        // �� ���� ���� ���θ� �����Ϳ� ����
        _data._isGateOpen = _castleGate.enabled;

        // ������ ������ Json�� �����ϴ� �۾�
        MapDataPackage._mapData._forest._dataStruct = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}