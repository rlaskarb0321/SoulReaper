using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDataPackage : DataApply, IDataApply
{
    [Header("=== Hierarchy ===")]
    public Transform[] _playerBody;
    public Image _hpFill;
    public TMP_Text _hpText;
    public Image _mpFill;
    public TMP_Text _mpText;
    public TMP_Text _seedCount;
    public TMP_Text _mapName;
    public TMP_Text _soulCount;

    [HideInInspector]
    public static CharacterData _cDataInstance; // 저장된 플레이어 데이터를 이곳에 입력시킴

    // Field
    private CharacterData.CData _data;
    private PlayerData _playerData;
    private PlayerMove_1 _playerMove;

    private void Awake()
    {
        _data = _cDataInstance._characterData;
        _playerData = _playerBody[0].GetComponent<PlayerData>();
        _playerMove = _playerBody[0].GetComponent<PlayerMove_1>();

        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // 플레이어와 쫓아다니는 캠 동시에 데이터에 적혀있던 위치로 옮기기
        _playerBody[0].rotation = _data._rot;
        _playerBody[0].position = new Vector3(_data._pos.x, _data._pos.y, _data._pos.z);
        _playerMove.TeleportPlayer(_playerBody[0], false);

        // 플레이어의 체력과 마나를 데이터에 적혀있던대로 수정
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, _data._currHP, _data._maxHP, false);
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.MP, _data._currMP, _data._maxMP, false);

        _seedCount.text = $"X {_data._seedCount}";

        // 데이터에 적혀있던 대로 가진 소울의 양을 수정
        _soulCount.text = $"X {_data._soulCount}";
        _playerData._soulCount = _data._soulCount;
    }

    public override void EditMapData()
    {
        print("Edit Character Data");

        // 플레이어의 체력, 마나 데이터 변경
        _data._currHP = _playerData._currHP;
        _data._maxHP = _playerData._maxHP;
        _data._currMP = _playerData._currMP;
        _data._maxMP = _playerData._maxMP;
        _data._soulCount = _playerData._soulCount;
        _data._basicAtkDamage = _playerData._basicAtkDamage;

        // 변경한 데이터들 저장
        _cDataInstance._characterData = _data;
        DataManage.SaveCData(_cDataInstance, "TestCData");
    }
}
