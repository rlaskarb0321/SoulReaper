using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleARoomDataApply : DataApply, IDataApply
{
    [Header("=== Hierarchy ===")]

    // Field
    private CastleARoom.RoomData _data;

    private void Awake()
    {
        _data = MapDataPackage._mapData._castleA._data;
        ApplyData();
    }

    public void ApplyData()
    {

    }

    public override void EditMapData()
    {

    }
}
