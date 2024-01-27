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

    [SerializeField]
    private Transform _buffEffectPos;

    // Field
    private BuffData _serverBuffData; // ���� ���� �����͸� ����
    private List<PlayerBuff> _localBuffData; // ��Ÿ�� ���� ��ü ������
    private List<float> _localBuffRemainDur; // ��Ÿ�� ���� ���ӽð� ������

    private void Awake()
    {
        _serverBuffData = DataManage.LoadBData("TestBData");
        _localBuffData = new List<PlayerBuff>();
        _localBuffRemainDur = new List<float>();

        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < _serverBuffData._buffDataList.Count; i++)
        {
            PlayerBuff dataBuff = _serverBuffData._buffDataList[i];
            BuffICon buffICon = Instantiate(_buffICon, _buffContainer);
            float duration = _serverBuffData._remainDurList[i];

            buffICon._buffImamge.sprite = dataBuff.BuffImg;
            buffICon.InitBuff(dataBuff, true);
            dataBuff.RemainBuffDur = duration;
            dataBuff.BuffPlayer();
            StartCoroutine(ManageBuff(dataBuff, buffICon));
        }
    }

    public override void EditData()
    {
        // print("Edit Buff Data");

        // ������ ��Ÿ�� �����͸� ���� �����Ϳ� �����
        _serverBuffData._buffDataList = _localBuffData;
        _serverBuffData._remainDurList = _localBuffRemainDur;

        DataManage.SaveBData(_serverBuffData, "TestBData");
    }

    /// <summary>
    /// ĳ���Ϳ��� buff �� �ִ� �޼���
    /// </summary>
    /// <param name="buff"></param>
    public void BuffPlayer(PlayerBuff buff)
    {
        PlayerBuff alreadyBuff = CheckAlreadyBuff(buff);
        if (alreadyBuff != null)
        {
            // print("Dupli");
            alreadyBuff.RemainBuffDur = alreadyBuff.BuffDur;
            return;
        }

        BuffICon buffICon = Instantiate(_buffICon, _buffContainer); // ���� �������� �����̳� �ؿ� ����
        BuffEffect buffEffect;

        if (buff._effect != null)
            buffEffect = Instantiate(buff._effect.GetComponent<BuffEffect>());

        buffICon.InitBuff(buff, false);
        buffICon._buffImamge.sprite = buff.BuffImg; // ���� ������ �ٲٱ�
        buff.BuffPlayer(); // ���� �ֱ�
        StartCoroutine(ManageBuff(buff, buffICon));
    }

    private PlayerBuff CheckAlreadyBuff(PlayerBuff buff)
    {
        for (int i = 0; i < _localBuffData.Count; i++)
        {
            string localBuffName = _localBuffData[i].BuffName;
            if (localBuffName.Equals(buff.BuffName))
            {
                return _localBuffData[i];
            }
        }

        return null;
    }

    private IEnumerator ManageBuff(PlayerBuff buff, BuffICon buffICon)
    {
        _localBuffData.Add(buff);
        _localBuffRemainDur.Add(buff.RemainBuffDur);

        int index;
        while (buff.RemainBuffDur > 0.0f)
        {
            buffICon._durationText.text = ((int)buff.RemainBuffDur).ToString();
            yield return null;

            buff.RemainBuffDur -= Time.deltaTime;
            index = _localBuffData.FindIndex(item => item.BuffName.Equals(buff.BuffName));
            _localBuffData[index] = buff; // ��Ÿ�� ���� ������ ����
            _localBuffRemainDur[index] = buff.RemainBuffDur; // ��Ÿ�� ���� ���ӽð� ������ ����
            EditData();
        }

        index = _localBuffData.FindIndex(item => item.BuffName.Equals(buff.BuffName));
        _localBuffData.Remove(buff);
        _localBuffRemainDur.RemoveAt(index);
        Destroy(buffICon.gameObject);
        buff.ResetBuff();
        EditData();
    }
}
