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
            HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None       // ȭ���� ����
            )
        {
            _isScrollGet = isScrollGet;
            _flowerState = flowerState;
        }

        public bool _isScrollGet;
        public HealthPlant.eFlowerState _flowerState;
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
        public RoomData
            (
            bool isA1RoomClear = false,                                                     // A1 �� Ŭ���� ����
            bool isA2RoomClear = false,                                                     // A2 �� Ŭ���� ����
            bool isYellowKeyGet = false,                                                    // ���ο� Ű ���� ����
            bool isA3RoomClear = false,                                                     // A3 �� Ŭ���� ����
            bool isA4RoomClear = false,                                                     // A4 �� Ŭ���� ����
            HealthPlant.eFlowerState a4FlowerState = HealthPlant.eFlowerState.None,         // A4 �濡 �ִ� ȭ���� ����
            bool isVictimYes = false                                                        // ����ڿ� ��ȭ���� ���� ���� �³��ߴ���
            )
        {
            _isA1RoomClear = isA1RoomClear;
            _isA2RoomClear = isA2RoomClear;
            _isYellowKeyGet = isYellowKeyGet;
            _isA3RoomClear = isA3RoomClear;
            _isA4RoomClear = isA4RoomClear;
            _a4FlowerState = a4FlowerState;
            _isVictimYes = isVictimYes;
        }

        public bool _isA1RoomClear;
        public bool _isA2RoomClear;
        public bool _isYellowKeyGet;
        public bool _isA3RoomClear;
        public bool _isA4RoomClear;
        public HealthPlant.eFlowerState _a4FlowerState;
        public bool _isVictimYes;
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
        public RoomData
            (
            bool isB1RoomClear = false,                                                         // B1 �� Ŭ���� ����
            HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None,               // B2 �� ȭ���� ����
            bool isVictimYes = false                                                            // ����ڿ� ��ȭ���� ���� ���� �³�����
            )
        {
            _isB1RoomClear = isB1RoomClear;
            _flowerState = flowerState;
            _isVictimYes = isVictimYes;
        }

        public bool _isB1RoomClear;
        public HealthPlant.eFlowerState _flowerState;
        public bool _isVictimYes;
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
        public RoomData
            (
            bool isWebDestruct = false,                                                         // �Ա��� ���� �Ź����� �ı��ߴ���
            bool[] bossEncounterPhase = null,                                                   // ���� ����ܰ�
            HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None,               // Ȧ���ִ� ȭ���� ����
            bool isUnlock = false                                                               // B ������ ���� ���� ��������
            )
        {
            _isWebDestruct = isWebDestruct;
            _bossEncounterPhase = bossEncounterPhase;
            _flowerState = flowerState;
            _isUnlock = isUnlock;
        }

        public bool _isWebDestruct;
        public bool[] _bossEncounterPhase;
        public HealthPlant.eFlowerState _flowerState;
        public bool _isUnlock;
    }

    public RoomData _data;

    public CastleHall()
    {
        _data = new RoomData(false, new bool[ConstData.TOTAL_BOSS_ENOCOUNTER_PHASE_COUNT], HealthPlant.eFlowerState.None);
    }

    public CastleHall(RoomData data)
    {
        _data = data;
    }
}
#endregion Castle Hall Data