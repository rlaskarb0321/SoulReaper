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
    public GameObject _toShrine;
    public GameObject _shelf;

    [Header("=== A3 Room ===")]
    public VictimUrnRoom _a3Room;
    public GameObject[] _victimUrns;
    public GameObject _portal;
    public GameObject[] _doorSide;
    public GameObject _seal;

    [Header("=== A4 Room ===")]
    public GameObject _a4Ladder;
    public BoxCollider _a4EntryTrigger;
    public HealthPlant _healthPlant;
    public GameObject _victimReal;
    public GameObject _rightUnseal;
    public GameObject _rightFire;
    public GameObject _victim;

    // Field
    private CastleARoom.RoomData _data;
    private CastleHall.RoomData _hallData;
    private enum eRoomNumber { A1, A2_Key, A2_1, }

    private void Awake()
    {
        _data = MapDataPackage._mapData._castleA._data;
        _hallData = MapDataPackage._mapData._castleHall._data;
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // �ҷ��� ������, A1�� ȭ���� ���¿����� �� Obj �ٲٱ�
        if (_data._isA1RoomClear)
        {
            _braziers[(int)eRoomNumber.A1].Ignite();
            _toShrine.SetActive(true);
            _shelf.GetComponent<Animator>().enabled = true;
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

        // ����ڿ� ��ȭ���� Yes�� ����������
        if (_data._isVictimYes)
        {
            if (_hallData._bossEncounterPhase[(int)ConstData.eBossEncounterPhase.Induce])
            {
                // Induce ������, ������������ �� ���� Ʈ���Ÿ� ��
                _rightUnseal.SetActive(false);
            }
            else
            {
                // ����� �������� Induce�� �� ������, ���� Ʈ���Ŵ� �ǵ��� �ʱ�
                _rightUnseal.SetActive(true);
            }

            // �� Ű�� ����� ����
            _rightFire.SetActive(true);
            _victim.SetActive(false);
        }
    }

    public override void EditData()
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

        // a4 �� ȭ���� ����
        _data._a4FlowerState = _healthPlant.FlowerState;

        // ����ڿ� ��ȭ���� ���� ���� ����
        if (!_victimReal.activeSelf)
        {
            _data._isVictimYes = true;
        }

        // ������ �����͸� Json�� ����
        MapDataPackage._mapData._castleA._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
