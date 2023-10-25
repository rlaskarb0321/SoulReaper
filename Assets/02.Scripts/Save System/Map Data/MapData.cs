using System;


/// <summary>
/// �� �����͵��� ���� �����ϴ� Ŭ����
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
// �� �� �����͸� �����ϴ� ��ü
[Serializable]
public class ForestMap
{
    [Serializable]
    public struct ForestData
    {
        public ForestData
            (
            bool isScrollGet = false,                                                   // ������ �о����� ����
            HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None,       // ȭ���� ����
            bool isShrineUnSealed = false,                                              // ����Ա� ���� ���� ����
            bool isShrineGet = false                                                    // ��� ���� ȹ��
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