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
    private BuffData _serverBuffData; // 게임에서 저장된 버프 데이터를 불러옴
    private List<BuffData.BData> _localBuffData;
    private WaitForSeconds _ws;
    private bool _testBool;

    private void Awake()
    {
        _serverBuffData = DataManage.LoadBData("TestBData");

        _ws = new WaitForSeconds(1.0f);
        _localBuffData = new List<BuffData.BData>();
        StartCoroutine(ApplyData());
    }

    public IEnumerator ApplyData()
    {
        yield return new WaitForSeconds(0.1f);
        _testBool = true;

        for (int i = 0; i < _serverBuffData._dataList.Count; i++)
        {
            PlayerBuff dataBuff = _serverBuffData._dataList[i]._buff;
            int duration = _serverBuffData._dataList[i]._duration;
            BuffData.BData bData = new BuffData.BData(dataBuff, duration);
            BuffICon buffIcon = Instantiate(_buffICon, _buffContainer);

            buffIcon._buffImamge.sprite = dataBuff.BuffImg; // 버프 아이콘 바꾸기
            dataBuff.BuffPlayer();
            StartCoroutine(ManageBuff(bData, buffIcon, buffIcon._durationText));
        }

        _testBool = false;
    }

    public override void EditData()
    {
        if (_testBool)
        {
            return;
        }

        // print("Edit Buff Data");

        // 변경한 데이터들 입력 후 저장
        _serverBuffData._dataList = _localBuffData;
        DataManage.SaveBData(_serverBuffData, "TestBData");
    }

    /// <summary>
    /// 캐릭터에게 buff 를 주는 메서드
    /// </summary>
    /// <param name="buff"></param>
    public void BuffPlayer(PlayerBuff buff)
    {
        // 이미 같은 버프가 걸려있는지 확인 후, 걸려있으면 시간만 초기값으로 갱신시켜줌
        PlayerBuff alreadyBuff = CheckAlreadyBuff(buff.BuffName);
        if (alreadyBuff != null)
        {
            print("Dupli");
            alreadyBuff.RemainBuffDur = alreadyBuff.BuffDur;
            return;
        }

        BuffICon buffICon = Instantiate(_buffICon, _buffContainer); // 버프 아이콘을 컨테이너 밑에 생성
        BuffData.BData bData = new BuffData.BData(buff, (int)buff.RemainBuffDur);

        buffICon._buffImamge.sprite = buff.BuffImg; // 버프 아이콘 바꾸기
        buff.BuffPlayer(); // 버프 주기

        StartCoroutine(ManageBuff(bData, buffICon, buffICon._durationText));
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

    private IEnumerator ManageBuff(BuffData.BData bData, BuffICon icon, TMP_Text text)
    {
        _localBuffData.Add(bData);
        int index;
        while (bData._duration > 0.0f)
        {
            text.text = bData._duration.ToString();
            EditData();
            yield return _ws;

            bData._duration--;
            index = _localBuffData.FindIndex(item => item._buff.BuffName.Equals(bData._buff.BuffName));
            _localBuffData[index] = bData; 
        }

        _localBuffData.Remove(bData);
        Destroy(icon.gameObject);
        bData._buff.ResetBuff();
        EditData();
    }
}
