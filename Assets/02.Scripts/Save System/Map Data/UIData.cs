using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIData : MonoBehaviour
{
    [Header("=== Hierarchy ===")]
    public Transform[] _playerBody;
    public Image _hpFill;
    public TMP_Text _hpText;
    public Image _mpFill;
    public TMP_Text _mpText;
    public TMP_Text _seedCount;
    public TMP_Text _mapName;

    [Header("=== Data ===")]
    public CharacterData _dataInstance;
    
    [HideInInspector]
    public CData _cdata;

    private void Awake()
    {
        _dataInstance = DataManage.LoadCData("TestCData");
        _cdata = _dataInstance._characterData;

        ApplyData();
    }

    private void ApplyData()
    {
        for (int i = 0; i < _playerBody.Length; i++)
        {
            _playerBody[i].position = _cdata._pos;
        }

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Hp, _cdata._currHp, _cdata._maxHp);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Mp, _cdata._currMp, _cdata._maxMp);

        _seedCount.text = $"X {_cdata._seedCount}";
        _mapName.text = _cdata._mapName;
    }
}
