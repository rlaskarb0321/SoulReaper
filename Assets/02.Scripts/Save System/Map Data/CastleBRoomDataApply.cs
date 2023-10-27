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

        // ������ ������ b1 �� Ŭ�������� ��
        if (_data._isB1RoomClear)
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

        // ������ ������ b2 �� Ŭ���� ���� ��
        if (_data._isB2RoomClear)
        {
            // Ʈ���Ų��� ��ٸ�Ű��
            _b2EntryTrigger.enabled = false;
            _b2Ladder.SetActive(true);
        }

        // ����ڿ� ��ȭ���� Yes�� �������� ��
        if (_data._isVictimYes)
        {
            if (_hallData._bossEncounterPhase[(int)ConstData.eBossEncounterPhase.Rational])
            {
                // Induce ������, ������������ �� ���� Ʈ���Ÿ� ��
                _leftUnseal.SetActive(false);
            }
            else
            {
                // ����� �������� Induce�� �� ������, ���� Ʈ���Ŵ� �ǵ��� �ʱ�
                _leftUnseal.SetActive(true);
            }

            // �� Ű�� ����� ����
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

        // ������ �����͸� Json�� ����
        MapDataPackage._mapData._castleB._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
