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

    [Header("=== ��Ȱ ����Ʈ ===")]
    public GameObject _revivePos;
    public SkinnedMeshRenderer _dummyPlayer;
    public Material _playerFadeMat;

    // ��� ������ �����ڿ��� ����� ������ ����ü
    private ForestMap.ForestData _data;

    private void Awake()
    {
        CharacterData.CData cData = CharacterDataPackage._cDataInstance._characterData;
        Material fadeMat = Instantiate(_playerFadeMat);
        Color color = fadeMat.color;

        // ���� ĳ���ʹ� ����ÿ��� ���̵��� ��Ű�� �� �ܿ��� ���µ� ���̰� �Ѵ�
        color.a = 0.0f;
        fadeMat.color = color;
        _dummyPlayer.material = fadeMat;

        // ĳ���Ͱ� �׾ ���� ���� �ҷ��°��
        if (cData._isPlayerDead)
        {
            // ĳ���� �����Ϳ� ��ġ�� ȸ���� ��Ȱ������ ������ �ű��
            cData._pos = _revivePos.transform.position;
            cData._rot = _dummyPlayer.transform.rotation;
            CharacterDataPackage._cDataInstance._characterData = cData;
            DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");

            // �� ������ �������� ���� ĳ���͸� �ް�, ���� ĳ���͸� ���̵� �� ��Ų��.
            StartCoroutine(FadeInPlayer(fadeMat, cData));
        }

        // ���� ������ �ο�
        _data = MapDataPackage._mapData._forest._dataStruct;
        StartCoroutine(ApplyData());
    }

    private IEnumerator FadeInPlayer(Material fadeMat, CharacterData.CData cData)
    {
        Color color = fadeMat.color;
        while (fadeMat.color.a < 1.0f)
        {
            color.a += Time.deltaTime * 0.5f;
            fadeMat.color = color;
            _dummyPlayer.material = fadeMat;
            print("alpha fade in");
            yield return null;
        }

        _dummyPlayer.gameObject.SetActive(false);
        UIScene._instance._stat.gameObject.SetActive(true);
        cData._isPlayerDead = false;
        CharacterDataPackage._cDataInstance._characterData = cData;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
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

        // ������ ������ Json�� �����ϴ� �۾�
        MapDataPackage._mapData._forest._dataStruct = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}