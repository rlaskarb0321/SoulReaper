using System;


/// <summary>
/// �� �����͵��� ���� �����ϴ� Ŭ����
/// </summary>
[Serializable]
public class MapData
{
    public ForestMap _forest;
    public CastleMap _castle;
}

/// <summary>
/// �� �� �����͸� �����ϴ� ��ü
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
/// �� �� ������ ����ü
/// </summary>
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
/// �� �� �����͸� �����ϴ� ��ü
/// </summary>
[Serializable]
public class CastleMap
{
    // �� enum ���� Castle Map ������ ���� ��ũ��Ʈ�� �ű���
    public enum eBossEncounterLevel { FirstMeet, Induce, Rationalize, } // ���� ���� �ܰ�
    public enum eARoomClear { A1, Shrine, A2, A3, A4, } // �ش� �ε�����°�� ���� Ŭ�����ߴ���
    public enum eBRoomClear { B1, B2, } // �ش� �ε�����°�� ���� Ŭ�����ߴ���

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
/// �� �� ������ ����ü
/// </summary>
[Serializable]
public struct CastleData
{
    public CastleData
        (
        bool isEntryClear = false,                                                  // Entry ���� Ŭ�����ߴ���
        bool[] isEncounteredBoss = null,                                            // ������ ����ܰ迡 �´� ������ �÷��� �ߴ���
        bool isA1Clear = false                                                      // A1 ���� Ŭ���� �ߴ��� (����� Ǯ������)
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