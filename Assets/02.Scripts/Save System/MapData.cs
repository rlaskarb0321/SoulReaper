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
    public ForestMap
        (bool isScrollGet = false,                                                  // ������ �о����� ����
        HealthPlant.eFlowerState flowerState = HealthPlant.eFlowerState.None,       // ȭ���� ����
        bool isShrineUnSealed = false,                                              // ����Ա� ���� ���� ����
        bool isShrineGet = false)                                                   // ��� ���� ȹ�� ����
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
/// �� �� �����͸� �����ϴ� ��ü
/// </summary>
[Serializable]
public class CastleMap
{
    public CastleMap(
        bool isEntryClear = false,                                                  // Entry ���� Ŭ�����ߴ���
        bool isPlayerFirstMeetBoss = false,                                         // ������ ù ���츦 �ߴ���
        bool isA1Clear = false)                                                     // A1 ���� Ŭ���� �ߴ��� (����� Ǯ������)
    {
        _isEntryClear = isEntryClear;
        _isPlayerFirstMeetBoss = isPlayerFirstMeetBoss;
        _isA1Clear = isA1Clear;
    }

    public bool _isEntryClear = false;
    public bool _isPlayerFirstMeetBoss = false;
    public bool _isA1Clear = false;
}