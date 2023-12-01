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

    [Header("=== 부활 포인트 ===")]
    public GameObject _revivePos;
    public SkinnedMeshRenderer _dummyPlayer;
    public Material _playerFadeMat;

    // 상부 데이터 관리자에게 제출용 데이터 구조체
    private ForestMap.ForestData _data;

    private void Awake()
    {
        CharacterData.CData cData = CharacterDataPackage._cDataInstance._characterData;
        Material fadeMat = Instantiate(_playerFadeMat);
        Color color = fadeMat.color;

        // 더미 캐릭터는 사망시에만 페이드인 시키고 그 외에는 없는듯 보이게 한다
        color.a = 0.0f;
        fadeMat.color = color;
        _dummyPlayer.material = fadeMat;

        // 캐릭터가 죽어서 씬을 새로 불러온경우
        if (cData._isPlayerDead)
        {
            // 캐릭터 데이터에 위치와 회전을 부활포지션 값으로 옮기기
            cData._pos = _revivePos.transform.position;
            cData._rot = _dummyPlayer.transform.rotation;
            CharacterDataPackage._cDataInstance._characterData = cData;
            DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");

            // 맵 데이터 관리에선 꺼진 캐릭터를 받고, 더미 캐릭터를 페이드 인 시킨다.
            StartCoroutine(FadeInPlayer(fadeMat, cData));
        }

        // 게임 데이터 부여
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

        // 데이터 수정후 Json에 저장하는 작업
        MapDataPackage._mapData._forest._dataStruct = _data;
        DataManage.SaveMData(MapDataPackage._mapData, "TestMData");
    }
}