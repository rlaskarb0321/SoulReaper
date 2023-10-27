using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleBRoomDataApply : DataApply, IDataApply
{
    [Header("------ Hierarchy ------")]
    [Header("=== B1 Room ===")]
    public VictimUrnRoom _b1Room;
    public GameObject[] _victimUrns;
    public GameObject _portal;
    public GameObject[] _doorSide;
    public GameObject _seal;

    [Header("=== B2 Room ===")]
    public GameObject _b2Ladder;
    public BoxCollider _b2EntryTrigger;
    public HealthPlant _healthPlant;
    public GameObject _victimReal;
    public GameObject _leftUnseal;
    public GameObject _leftFire;
    public GameObject _victim;
    public BossGate _bossGate;

    // Field
    private CastleBRoom.RoomData _data;
    private CastleHall.RoomData _hallData;

    private void Awake()
    {
        _data = MapDataPackage._mapData._castleB._data;
        _hallData = MapDataPackage._mapData._castleHall._data;
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // 데이터 상으로 b1 방 클리어했을 때
        if (_data._isB1RoomClear)
        {
            // 희생자 항아리들 끄기
            for (int i = 0; i < _victimUrns.Length; i++)
            {
                _victimUrns[i].SetActive(false);
            }

            // 문 내리고 포탈 활성화
            for (int i = 0; i < _doorSide.Length; i++)
            {
                _doorSide[i].SetActive(false);
            }
            _portal.SetActive(true);
            _seal.SetActive(false);
        }

        // 데이터 상으로 b2 방 클리어 했을 때
        if (_data._isB2RoomClear)
        {
            // 트리거끄고 사다리키기
            _b2EntryTrigger.enabled = false;
            _b2Ladder.SetActive(true);
        }

        // 희생자와 대화에서 Yes를 선택했을 때
        if (_data._isVictimYes)
        {
            if (_hallData._bossEncounterPhase[(int)ConstData.eBossEncounterPhase.Rational])
            {
                // Induce 봤으니, 좌측봉인해제 후 연출 트리거를 끔
                _leftUnseal.SetActive(false);
            }
            else
            {
                // 희생자 보내놓고 Induce는 안 봤으니, 연출 트리거는 건들지 않기
                _leftUnseal.SetActive(true);
            }

            // 불 키고 희생자 끄고
            _leftFire.SetActive(true);
            _victim.SetActive(false);
            _bossGate.CanInteract();
        }
    }

    public override void EditMapData()
    {
        print("Castle B Data Apply");

        if (_b1Room._sealCount == 0)
        {
            _data._isB1RoomClear = true;
        }

        if (_b2Ladder.activeSelf)
        {
            _data._isB2RoomClear = true;
        }

        _data._flowerState = _healthPlant.FlowerState;

        if (!_victimReal.activeSelf)
        {
            _data._isVictimYes = true;
        }

        // 수정한 데이터를 Json에 저장
        MapDataPackage._mapData._castleB._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
