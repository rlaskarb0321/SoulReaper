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
        // �̹� ���� ������ �ɷ��ִ��� Ȯ�� ��, �ɷ������� �ð��� �ʱⰪ���� ���Ž�����
        PlayerBuff alreadyBuff = CheckAlreadyBuff(buff.BuffName);
        if (alreadyBuff != null)
        {
            alreadyBuff.RemainBuffDur = alreadyBuff.BuffDur;
            return;
        }

        BuffICon buffICon = Instantiate(_buffICon, _buffContainer); // ���� �������� �����̳� �ؿ� ����
        buffICon._buffImamge.sprite = buff.BuffImg; // ���� ������ �ٲٱ�
        buff.BuffPlayer(); // ���� �ֱ�
        StartCoroutine(buff.DecreaseBuffDur(buffICon._durationText)); // ���� ���ӽð� �ؽ�Ʈ �ٲٱ�
        StartCoroutine(ManageBuff(buff, buffICon)); // ���� ����Ʈ�� �����ϱ�
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
