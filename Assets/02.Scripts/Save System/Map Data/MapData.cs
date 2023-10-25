using System;


/// <summary>
/// 맵 데이터들을 전부 관리하는 클래스
/// </summary>
[Serializable]
public class MapData
{
    public ForestMap _forest;
    public CastleARoom _castleA;
    public CastleBRoom _castleB;
    public CastleHall _castleHall;

    public MapData()
    {
        _forest = new ForestMap();
        _castleA = new CastleARoom();
        _castleB = new CastleBRoom();
        _castleHall = new CastleHall();
    }
}

#region Forest Map Data
// 숲 맵 데이터를 저장하는 객체
[Serializable]
public class ForestMap
{
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
            _flowerState = flowerState;
            _isShrineUnSealed = isShrineUnSealed;
            _isShrineGet = isShrineGet;
        }

        public bool _isScrollGet;
        public HealthPlant.eFlowerState _flowerState;
        public bool _isShrineUnSealed;
        public bool _isShrineGet;
    }

    public ForestData _dataStruct;

    public ForestMap()
    {
        _dataStruct = new ForestData();
    }

    public ForestMap(ForestData data)
    {
        _dataStruct = data;
    }
}
 #endregion Forest Map Data


#region Castle A Room Data
[Serializable]
public class CastleARoom
{
    [Serializable]
    public struct RoomData
    {

    }

    public RoomData _data;

    public CastleARoom()
    {
        _data = new RoomData();
    }

    public CastleARoom(RoomData data)
    {
        _data = data;
    }
}
#endregion Castle A Room Data

#region Castle B Room Data
[Serializable]
public class CastleBRoom
{
    [Serializable]
    public struct RoomData
    {

    }

    public RoomData _data;

    public CastleBRoom()
    {
        _data = new RoomData();
    }

    public CastleBRoom(RoomData data)
    {
        _data = data;
    }
}
#endregion Castle B Room Data


#region Castle Hall Data
[Serializable]
public class CastleHall
{
    [Serializable]
    public struct RoomData
    {

    }

    public RoomData _data;

    public CastleHall()
    {
        _data = new RoomData();
    }

    public CastleHall(RoomData data)
    {
        _data = data;
    }
}
#endregion Castle Hall Data