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

        // 변경한 데이터들 입력 후 저장
        _serverBuffData._dataList = _localBuffData;
        DataManage.SaveBData(_serverBuffData, "TestBData");
    }

    public void BuffPlayer(PlayerBuff buff)
    {
        // 이미 같은 버프가 걸려있는지 확인 후, 걸려있으면 시간만 초기값으로 갱신시켜줌
        PlayerBuff alreadyBuff = CheckAlreadyBuff(buff.BuffName);
        if (alreadyBuff != null)
        {
            alreadyBuff.RemainBuffDur = alreadyBuff.BuffDur;
            return;
        }

        BuffICon buffICon = Instantiate(_buffICon, _buffContainer); // 버프 아이콘을 컨테이너 밑에 생성
        BuffData.BData bData = new BuffData.BData(buff, (int)buff.BuffDur);

        buffICon._buffImamge.sprite = buff.BuffImg; // 버프 아이콘 바꾸기
        buff.BuffPlayer(); // 버프 주기
        StartCoroutine(ManageBuff(bData, buffICon)); // 버프 리스트로 관리하기
        StartCoroutine(DecreaseBuffDur(buffICon._durationText, bData)); // 버프 지속시간 텍스트 바꾸기
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
