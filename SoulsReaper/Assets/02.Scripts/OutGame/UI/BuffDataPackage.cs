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
    private BuffData _serverBuffData; // 로컬 버프 데이터를 저장
    private List<PlayerBuff> _localBuffData; // 런타임 버프 객체 데이터
    private List<float> _localBuffRemainDur; // 런타임 버프 지속시간 데이터

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

        // 수정된 런타임 데이터를 로컬 데이터에 덮어씌움
        _serverBuffData._buffDataList = _localBuffData;
        _serverBuffData._remainDurList = _localBuffRemainDur;

        DataManage.SaveBData(_serverBuffData, "TestBData");
    }

    /// <summary>
    /// 캐릭터에게 buff 를 주는 메서드
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

        BuffICon buffICon = Instantiate(_buffICon, _buffContainer); // 버프 아이콘을 컨테이너 밑에 생성
        BuffEffect buffEffect;

        if (buff._effect != null)
            buffEffect = Instantiate(buff._effect.GetComponent<BuffEffect>());

        buffICon.InitBuff(buff, false);
        buffICon._buffImamge.sprite = buff.BuffImg; // 버프 아이콘 바꾸기
        buff.BuffPlayer(); // 버프 주기
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
            _localBuffData[index] = buff; // 런타임 버프 데이터 수정
            _localBuffRemainDur[index] = buff.RemainBuffDur; // 런타임 버프 지속시간 데이터 수정
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
