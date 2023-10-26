using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleARoomDataApply : DataApply, IDataApply
{
    [Header("------ Hierarchy ------")]
    [Header("=== A1 & A2 Room ===")]
    public Flammable[] _braziers;
    public GameObject _yelloKeyMesh;
    public Key _yelloKey;

    [Header("=== A3 Room ===")]
    public VictimUrnRoom _a3Room;
    public GameObject[] _victimUrns;
    public GameObject _portal;
    public GameObject[] _doorSide;
    public GameObject _seal;

    [Header("=== A4 Room ===")]
    public GameObject _a4Ladder;
    public BoxCollider _a4EntryTrigger;

    // Field
    private CastleARoom.RoomData _data;
    private enum eRoomNumber { A1, A2_Key, A2_1, }

    private void Awake()
    {
        _data = MapDataPackage._mapData._castleA._data;
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // 불러온 데이터, A1의 화로의 상태에따라 씬 Obj 바꾸기
        if (_data._isA1RoomClear)
        {
            _braziers[(int)eRoomNumber.A1].Ignite();
            // Shrine 입구 열기
        }

        // 불러온 데이터, A2의 화로의 상태에따라 씬 Obj 바꾸기
        if (_data._isA2RoomClear)
        {
            // 화로 두개 켜주고
            _braziers[(int)eRoomNumber.A2_Key].Ignite();
            _braziers[(int)eRoomNumber.A2_1].Ignite();

            // 키를 가져갈 수 있도록 설정
            _yelloKey.Reward();
        }

        // 불러온 데이터, 옐로우 키를 가져간 상태면 키 오브젝트 끄기
        if (_data._isYellowKeyGet)
        {
            _yelloKey.gameObject.SetActive(false);
        }

        // A3 방
        // 희생자 봉인 항아리가 있는 방 클리어 상태에 따른 Obj 변경
        if (_data._isA3RoomClear)
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

        // 미니 보스 트리거 끄기, 사다리 켜기
        if (_data._isA4RoomClear)
        {
            _a4EntryTrigger.enabled = false;
            _a4Ladder.SetActive(true);
        }
    }

    public override void EditMapData()
    {
        print("Castle A Data Apply");

        // A1에 있는 화로의 상태를 데이터에 저장
        // A1에 있는 특정 화로가 불타고있다면 A2 방 클리어한 것
        if (_braziers[(int)eRoomNumber.A1]._fireState == Flammable.eFireState.Fire)
        {
            _data._isA1RoomClear = true;
        }

        // A2에 있는 화로의 상태를 데이터에 저장
        // A2에 있는 특정 화로가 불타고있다면 A2 방 클리어한 것
        if (_braziers[(int)eRoomNumber.A2_Key]._fireState == Flammable.eFireState.Fire)
        {
            _data._isA2RoomClear = true;
        }

        // A2에 있는 옐로우 키 가져감 ~을 저장
        if (!_yelloKeyMesh.activeSelf)
        {
            _data._isYellowKeyGet = true;
        }

        // A3에 있는 희생자 봉인 항아리가 전부 깨졌을 때 저장
        if (_a3Room._sealCount == 0)
        {
            _data._isA3RoomClear = true;
        }

        // A4룸 클리어해서 사다리가 활성화됬을 때 클리어 인정 후 저장
        if (_a4Ladder.activeSelf)
        {
            _data._isA4RoomClear = true;
        }

        // 수정한 데이터를 Json에 저장
        MapDataPackage._mapData._castleA._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
