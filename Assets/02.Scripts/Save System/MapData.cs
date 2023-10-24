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
    public ForestMap
        (bool isScrollGet = false,                                                  // 전서구 읽었는지 여부
        HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None,       // 화분의 상태
        bool isShrineUnSealed = false,                                              // 사당입구 봉인 해제 여부
        bool isShrineGet = false)                                                   // 사당 보상 획득 여부
    {
        _isScrollGet = isScrollGet;
        _state = flowerState;
        _isShrineUnSealed = isShrineUnSealed;
        _isShrineGet = isShrineGet;
    }

    public bool _isScrollGet = false;
    public HealthPlant.eFlowerState _state = HealthPlant.eFlowerState.None;
    public bool _isShrineUnSealed = false;
    public bool _isShrineGet = false;
}

/// <summary>
/// 성 맵 데이터를 저장하는 객체
/// </summary>
[Serializable]
public class CastleMap
{
    public CastleMap(
        bool isEntryClear = false,                                                  // Entry 방을 클리어했는지
        bool isPlayerFirstMeetBoss = false,                                         // 보스와 첫 조우를 했는지
        bool isA1Clear = false)                                                     // A1 방을 클리어 했는지 (비밀을 풀었는지)
    {
        _isEntryClear = isEntryClear;
        _isPlayerFirstMeetBoss = isPlayerFirstMeetBoss;
        _isA1Clear = isA1Clear;
    }

    public bool _isEntryClear = false;
    public bool _isPlayerFirstMeetBoss = false;
    public bool _isA1Clear = false;
}