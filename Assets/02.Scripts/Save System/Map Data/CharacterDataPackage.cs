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

    [HideInInspector]
    public static CharacterData _characterData; // ����� �÷��̾� �����͸� �̰��� �Է½�Ŵ

    // Field
    private CharacterData.CData _data;
    private PlayerData _playerData;
    private PlayerMove_1 _playerMove;

    private void Awake()
    {
        _characterData = DataManage.LoadCData("TestCData");
        _data = _characterData._characterData;
        _playerData = _playerBody[0].GetComponent<PlayerData>();
        _playerMove = _playerBody[0].GetComponent<PlayerMove_1>();

        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        // �÷��̾�� �Ѿƴٴϴ� ķ ���ÿ� �����Ϳ� �����ִ� ��ġ�� �ű��
        _playerBody[0].rotation = _data._rot;
        _playerBody[0].position = new Vector3(_data._pos.x, _data._pos.y, _data._pos.z);
        _playerMove.TeleportPlayer(_playerBody[0]);

        // �÷��̾��� ü�°� ������ �����Ϳ� �����ִ���� ����
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
