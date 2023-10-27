using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIData : DataApply, IDataApply
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
    public CharacterData _characterData;

    // Field
    private CharacterData.CData _data;
    private PlayerData _playerData;

    private void Awake()
    {
        _characterData = DataManage.LoadCData("TestCData");
        _data = _characterData._characterData;
        _playerData = _playerBody[0].GetComponent<PlayerData>();

        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < _playerBody.Length; i++)
        {
            _playerBody[i].position = _data._pos;
        }

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Hp, _data._currHP, _data._maxHP, false);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Mp, _data._currMP, _data._maxMP, false);

        _seedCount.text = $"X {_data._seedCount}";
    }

    public override void EditMapData()
    {
        print("Edit Character Data");

        // �÷��̾��� ü��, ���� ������ ����
        _data._currHP = _playerData._currHP;
        _data._maxHP = _playerData._maxHP;
        _data._currMP = _playerData._currMP;
        _data._maxMP = _playerData._maxMP;

        // ������ �����͵� ����
        _characterData._characterData = _data;
        DataManage.SaveCData(_characterData, "TestCData");
    }
}
