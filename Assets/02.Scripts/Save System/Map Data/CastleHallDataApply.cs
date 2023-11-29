using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Castle ���� Hall ���� ������ �����ϴ� ��ũ��Ʈ
public class CastleHallDataApply : DataApply, IDataApply
{
    [Header("=== ���̷�Ű ===")]
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

        // �ҷ��� ������, �Ա� �Ź��� �ı� ���ο� ���� Obj ���� �ٲٱ�
        if (_data._isWebDestruct)
        {
            _spiderWeb.gameObject.SetActive(false);
        }

        // �ҷ��� ������, ���� ���� Cinemachine Ʈ������ enable ���� ���� ���� �ٲٱ�
        for (int i = 0; i < _data._bossEncounterPhase.Length; i++)
        {
            if (!_data._bossEncounterPhase[i])
                continue;
            if (i.Equals(0))
                _bossGateAnim.enabled = true;

            _bossEncounterPhase[i].enabled = !_data._bossEncounterPhase[i];
            _productionObjParent[i].SetActive(!_data._bossEncounterPhase[i]);
        }

        // �ҷ��� ������ ������ ���� ���� �ٲٱ�
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

        // �Ա��� �����ִ� �Ź����� ���¸� �����Ϳ� ����
        if (_spiderWeb._fireState == Flammable.eFireState.Fire)
        {
            _data._isWebDestruct = true;
        }

        // ������ ����ܰ迡 ���� Cinemachine �÷��� ���� �뵵�� Ʈ������ ���¸� �����Ϳ� ����
        for (int i = 0; i < _bossEncounterPhase.Length; i++)
        {
            if (_bossEncounterPhase[i].enabled)
                continue;

            _data._bossEncounterPhase[i] = true;
        }

        // ȸ�� ȭ���� ���¸� �����Ϳ� ����
        _data._flowerState = _healthPlant.FlowerState;

        if (!_bRoomKey.enabled)
        {
            _data._isUnlock = true;
        }

        // ������ ������ Json�� �����ϴ� �۾�
        MapDataPackage._mapData._castleHall._data = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}
