using System;


/// <summary>
/// 맵 데이터들을 전부 관리하는 클래스
/// </summary>
[Serializable]
public class MapData
{
    public ForestMap _forest;
    public CastleMap _castle;
}

/// <summary>
/// 숲 맵 데이터를 저장하는 객체
/// </summary>
[Serializable]
public class ForestMap
{
    public ForestData _data;

    public ForestMap()
    {
        _data = new ForestData();
    }

    public ForestMap(ForestData data)
    {
        _data = data;
    }
}

/// <summary>
/// 숲 맵 데이터 구조체
/// </summary>
[Serializable]
public struct ForestData
{
    public ForestData
        (
        bool isScrollGet = false,                                                   // 전서구 읽었는지 여부
        HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None,       // 화분의 상태
        bool isShrineUnSealed = false,                                              // 사당입구 봉인 해제 여부
        bool isShrineGet = false                                                    // 사당 보상 획득
        )
    {
        _isScrollGet = isScrollGet;
        _state = flowerState;
        _isShrineUnSealed = isShrineUnSealed;
        _isShrineGet = isShrineGet;
    }

    public bool _isScrollGet;
    public HealthPlant.eFlowerState _state;
    public bool _isShrineUnSealed;
    public bool _isShrineGet;
}




/// <summary>
/// 성 맵 데이터를 저장하는 객체
/// </summary>
[Serializable]
public class CastleMap
{
    // 이 enum 들은 Castle Map 데이터 전용 스크립트로 옮기자
    public enum eBossEncounterLevel { FirstMeet, Induce, Rationalize, } // 보스 조우 단계
    public enum eARoomClear { A1, Shrine, A2, A3, A4, } // 해당 인덱스번째의 방을 클리어했는지
    public enum eBRoomClear { B1, B2, } // 해당 인덱스번째의 방을 클리어했는지

    public CastleData _data;

    public CastleMap()
    {
        _data = new CastleData();
    }

    public CastleMap(CastleData data)
    {
        _data = data;
    }
}

/// <summary>
/// 성 맵 데이터 구조체
/// </summary>
[Serializable]
public struct CastleData
{
    public CastleData
        (
        bool isEntryClear = false,                                                  // Entry 방을 클리어했는지
        bool[] isEncounteredBoss = null,                                            // 보스와 조우단계에 맞는 연출을 플레이 했는지
        bool isA1Clear = false                                                      // A1 방을 클리어 했는지 (비밀을 풀었는지)
        )
    {
        _isEntryClear = isEntryClear;
        _isEncounteredBoss = isEncounteredBoss;
        _isA1Clear = isA1Clear;
    }

    public bool _isEntryClear;
    public bool[] _isEncounteredBoss;
    public bool _isA1Clear;
}