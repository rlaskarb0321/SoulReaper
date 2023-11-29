using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Castle 씬에 Hall 맵의 데이터 관리하는 스크립트
public class CastleHallDataApply : DataApply, IDataApply
{
    [Header("=== 하이러키 ===")]
    [SerializeField]
    private SpiderWeb _spiderWeb;

    [SerializeField]
    private BoxCollider[] _bossEncounterPhase;

    [SerializeField]
    private GameObject[] _productionObjParent;

    [SerializeField]
    private Animator _bossGateAnim;

    [SerializeField]
    private HealthPlant _healthPlant;

    [SerializeField]
    private BoxCollider _bRoomKey;

    // Field
    private CastleHall.RoomData _data;

    private void Awake()
    {
        _data = MapDataPackage._mapData._castleHall._data;
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // 불러온 데이터, 입구 거미줄 파괴 여부에 따라 Obj 상태 바꾸기
        if (_data._isWebDestruct)
        {
            _spiderWeb.gameObject.SetActive(false);
        }

        // 불러온 데이터, 보스 조우 Cinemachine 트리거의 enable 값에 따라 상태 바꾸기
        for (int i = 0; i < _data._bossEncounterPhase.Length; i++)
        {
            if (!_data._bossEncounterPhase[i])
                continue;
            if (i.Equals(0))
                _bossGateAnim.enabled = true;

            _bossEncounterPhase[i].enabled = !_data._bossEncounterPhase[i];
            _productionObjParent[i].SetActive(!_data._bossEncounterPhase[i]);
        }

        // 불러온 데이터 값으로 꽃의 상태 바꾸기
        switch (_data._flowerState)
        {
            case HealthPlant.eFlowerState.Bloom:
                _healthPlant.GrownPlant();
                break;
            case HealthPlant.eFlowerState.harvested:
                _healthPlant.HarvestPlant();
                break;
        }

        if (_data._isUnlock)
        {
            _bRoomKey.gameObject.SetActive(false);
        }
    }

    public override void EditData()
    {
        print("Hall Data Apply");

        // 입구를 막고있던 거미줄의 상태를 데이터에 저장
        if (_spiderWeb._fireState == Flammable.eFireState.Fire)
        {
            _data._isWebDestruct = true;
        }

        // 보스의 조우단계에 맞춰 Cinemachine 플레이 감지 용도인 트리거의 상태를 데이터에 저장
        for (int i = 0; i < _bossEncounterPhase.Length; i++)
        {
            if (_bossEncounterPhase[i].enabled)
                continue;

            _data._bossEncounterPhase[i] = true;
        }

        // 회복 화분의 상태를 데이터에 저장
        _data._flowerState = _healthPlant.FlowerState;

        if (!_bRoomKey.enabled)
        {
            _data._isUnlock = true;
        }

        // 데이터 수정후 Json에 저장하는 작업
        MapDataPackage._mapData._castleHall._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
