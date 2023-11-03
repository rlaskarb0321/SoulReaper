using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuffDataPackage : DataApply, IDataApply
{
    [Header("=== Buff List ===")]
    [SerializeField]
    private Transform _buffContainer;

    [SerializeField]
    private BuffICon _buffICon;

    // Field
    private BuffData _serverBuffData; // ���ӿ��� ����� ���� �����͸� �ҷ���
    private List<BuffData.BData> _localBuffData;
    private PlayerData _playerData;
    private WaitForSeconds _ws;

    private void Awake()
    {
        _playerData = GetComponent<PlayerData>();
        _serverBuffData = DataManage.LoadBData("TestBData");

        _ws = new WaitForSeconds(1.0f);
        _localBuffData = new List<BuffData.BData>();
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

    }

    public override void EditData()
    {
        print("Edit Buff Data");

        // ������ �����͵� �Է� �� ����
        _serverBuffData._dataList = _localBuffData;
        DataManage.SaveBData(_serverBuffData, "TestBData");
    }

    public void BuffPlayer(PlayerBuff buff)
    {
        // �̹� ���� ������ �ɷ��ִ��� Ȯ�� ��, �ɷ������� �ð��� �ʱⰪ���� ���Ž�����
        PlayerBuff alreadyBuff = CheckAlreadyBuff(buff.BuffName);
        if (alreadyBuff != null)
        {
            alreadyBuff.RemainBuffDur = alreadyBuff.BuffDur;
            return;
        }

        BuffICon buffICon = Instantiate(_buffICon, _buffContainer); // ���� �������� �����̳� �ؿ� ����
        BuffData.BData bData = new BuffData.BData(buff, (int)buff.BuffDur);

        buffICon._buffImamge.sprite = buff.BuffImg; // ���� ������ �ٲٱ�
        buff.BuffPlayer(); // ���� �ֱ�
        StartCoroutine(ManageBuff(bData, buffICon)); // ���� ����Ʈ�� �����ϱ�
        StartCoroutine(DecreaseBuffDur(buffICon._durationText, bData)); // ���� ���ӽð� �ؽ�Ʈ �ٲٱ�
    }

    private PlayerBuff CheckAlreadyBuff(string buffName)
    {
        for (int i = 0; i < _localBuffData.Count; i++)
        {
            string buff = _localBuffData[i]._buff.BuffName;

            if (buff.Equals(buffName))
            {
                return _localBuffData[i]._buff;
            }
        }

        return null;
    }

    private IEnumerator ManageBuff(BuffData.BData buff, BuffICon icon)
    {
        _localBuffData.Add(buff);
        EditData();

        int testNum = 10;
        while (testNum > 0)
        {
            print((int)buff._duration);
            testNum--;
            yield return _ws;
        }

        yield return new WaitUntil(() => (int)buff._duration <= 0);

        Destroy(icon.gameObject);
        buff._buff.ResetBuff();
        _localBuffData.Remove(buff);
        EditData();
    }

    public IEnumerator DecreaseBuffDur(TMP_Text text, BuffData.BData buff)
    {
        int index = _localBuffData.IndexOf(buff);
        while (buff._duration > 0.0f)
        {
            text.text = buff._duration.ToString();
            yield return _ws;

            buff._duration -= 1;
            _localBuffData[index] = buff;
            EditData();
        }
    }
}
