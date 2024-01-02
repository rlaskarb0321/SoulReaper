using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestDataApply : DataApply, IDataApply
{
    [Header("=== 하이어러키 ===")]
    public GameObject _scrollMesh;
    public GameObject _scrollObj;
    public GameObject _sparrow;
    public HealthPlant _healthFlower;
    public MonsterBase _gateKeeper;
    public BoxCollider _castleGate;
    public GameObject _castleBattleICon;
    public BullInteract _bullInteract;

    [Header("=== 부활 포인트 ===")]
    public ReviveStone _reviveStone;

    // 상부 데이터 관리자에게 제출용 데이터 구조체
    private ForestMap.ForestData _data;

    private void Awake()
    {
        // 플레이어 캐릭터 사망했을 시 부활
        _reviveStone.Revive();

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

        // 스크롤의 획득 여부를 데이터에 저장
        if (!_scrollMesh.activeSelf)
        {
            _data._isScrollGet = true;
        }

        // 회복 화분의 상태를 데이터에 저장
        _data._flowerState = _healthFlower.FlowerState;

        // 문지기의 사망 여부를 데이터에 저장
        _data._isGateKeeperDead = _gateKeeper._state == MonsterBase.eMonsterState.Dead ? true : false;

        // 성 문의 열림 여부를 데이터에 저장
        _data._isGateOpen = _castleGate.enabled;

        // 데이터 수정후 Json에 저장하는 작업
        MapDataPackage._mapData._forest._dataStruct = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}