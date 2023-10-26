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

        // �ҷ��� ������, A1�� ȭ���� ���¿����� �� Obj �ٲٱ�
        if (_data._isA1RoomClear)
        {
            _braziers[(int)eRoomNumber.A1].Ignite();
            // Shrine �Ա� ����
        }

        // �ҷ��� ������, A2�� ȭ���� ���¿����� �� Obj �ٲٱ�
        if (_data._isA2RoomClear)
        {
            // ȭ�� �ΰ� ���ְ�
            _braziers[(int)eRoomNumber.A2_Key].Ignite();
            _braziers[(int)eRoomNumber.A2_1].Ignite();

            // Ű�� ������ �� �ֵ��� ����
            _yelloKey.Reward();
        }

        // �ҷ��� ������, ���ο� Ű�� ������ ���¸� Ű ������Ʈ ����
        if (_data._isYellowKeyGet)
        {
            _yelloKey.gameObject.SetActive(false);
        }

        // A3 ��
        // ����� ���� �׾Ƹ��� �ִ� �� Ŭ���� ���¿� ���� Obj ����
        if (_data._isA3RoomClear)
        {
            // ����� �׾Ƹ��� ����
            for (int i = 0; i < _victimUrns.Length; i++)
            {
                _victimUrns[i].SetActive(false);
            }

            // �� ������ ��Ż Ȱ��ȭ
            for (int i = 0; i < _doorSide.Length; i++)
            {
                _doorSide[i].SetActive(false);
            }
            _portal.SetActive(true);
            _seal.SetActive(false);
        }

        // �̴� ���� Ʈ���� ����, ��ٸ� �ѱ�
        if (_data._isA4RoomClear)
        {
            _a4EntryTrigger.enabled = false;
            _a4Ladder.SetActive(true);
        }
    }

    public override void EditMapData()
    {
        print("Castle A Data Apply");

        // A1�� �ִ� ȭ���� ���¸� �����Ϳ� ����
        // A1�� �ִ� Ư�� ȭ�ΰ� ��Ÿ���ִٸ� A2 �� Ŭ������ ��
        if (_braziers[(int)eRoomNumber.A1]._fireState == Flammable.eFireState.Fire)
        {
            _data._isA1RoomClear = true;
        }

        // A2�� �ִ� ȭ���� ���¸� �����Ϳ� ����
        // A2�� �ִ� Ư�� ȭ�ΰ� ��Ÿ���ִٸ� A2 �� Ŭ������ ��
        if (_braziers[(int)eRoomNumber.A2_Key]._fireState == Flammable.eFireState.Fire)
        {
            _data._isA2RoomClear = true;
        }

        // A2�� �ִ� ���ο� Ű ������ ~�� ����
        if (!_yelloKeyMesh.activeSelf)
        {
            _data._isYellowKeyGet = true;
        }

        // A3�� �ִ� ����� ���� �׾Ƹ��� ���� ������ �� ����
        if (_a3Room._sealCount == 0)
        {
            _data._isA3RoomClear = true;
        }

        // A4�� Ŭ�����ؼ� ��ٸ��� Ȱ��ȭ���� �� Ŭ���� ���� �� ����
        if (_a4Ladder.activeSelf)
        {
            _data._isA4RoomClear = true;
        }

        // ������ �����͸� Json�� ����
        MapDataPackage._mapData._castleA._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
