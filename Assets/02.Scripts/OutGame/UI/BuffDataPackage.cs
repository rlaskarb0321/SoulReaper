using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDataPackage : DataApply, IDataApply
{
    [Header("=== Buff List ===")]
    [SerializeField]
    private Transform _buffContainer;

    [SerializeField]
    private BuffICon _buffICon;

    // Field
    private PlayerData _playerData;
    private List<PlayerBuff> _buffList;

    private void Awake()
    {
        _playerData = GetComponent<PlayerData>();
        _buffList = new List<PlayerBuff>();
    }

    private void Update()
    {
        print(_buffList.Count);
    }

    public IEnumerator ApplyData()
    {
        yield return null;
    }

    public override void EditMapData()
    {

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
        buffICon._buffImamge.sprite = buff.BuffImg; // 버프 아이콘 바꾸기
        buff.BuffPlayer(); // 버프 주기
        StartCoroutine(buff.DecreaseBuffDur(buffICon._durationText)); // 버프 지속시간 텍스트 바꾸기
        StartCoroutine(ManageBuff(buff, buffICon)); // 버프 리스트로 관리하기
    }

    private PlayerBuff CheckAlreadyBuff(string buffName)
    {
        for (int i = 0; i < _buffList.Count; i++)
        {
            string buff = _buffList[i].BuffName;

            if (buff.Equals(buffName))
            {
                return _buffList[i];
            }
        }

        return null;
    }

    private IEnumerator ManageBuff(PlayerBuff buff, BuffICon icon)
    {
        _buffList.Add(buff);
        EditMapData();

        yield return new WaitUntil(() => (int)buff.RemainBuffDur <= 0);

        Destroy(icon.gameObject);
        _buffList.Remove(buff);
        buff.ResetBuff();
        EditMapData();
    }
}
