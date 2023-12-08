using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossRoom : QuestRoom
{
    [Header("=== Boss ===")]
    [SerializeField]
    private GameObject _boss;

    [SerializeField]
    private GameObject _dummyBoss;

    [SerializeField]
    private GameObject _battleICon;

    [SerializeField]
    private BGMChanger _bgmChanger;

    public bool _isDevelopMode;
    private PlayableDirector _playable;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
    }

    public override void SolveQuest()
    {
        // ���� �׿��� �� ȣ��, ������ ����
        print("hi");
    }

    /// <summary>
    /// �ƽ��� ������, ������ ���� => �ñ׳η� ������ �ٲٱ�, ��Ʋ������ ����, ���� ���� ���� ��¥ ���� Ű��
    /// </summary>
    public void StartBattle(AudioClip audioClip)
    {
        StartCoroutine(_bgmChanger.FadeOutClip(audioClip, 0.5f));

        _battleICon.gameObject.SetActive(true);
        _dummyBoss.SetActive(false);
        _boss.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;

        StartCoroutine(_bgmChanger.FadeOutClip(null, 1.0f));
        ProductionMgr.StartProduction(_playable);
        GetComponent<BoxCollider>().enabled = false;
    }
}
