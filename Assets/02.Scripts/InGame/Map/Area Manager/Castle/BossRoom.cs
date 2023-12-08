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
        // 보스 죽였을 때 호출, 게임의 엔딩
        print("hi");
    }

    /// <summary>
    /// 컷신이 끝나고, 전투가 시작 => 시그널로 비지엠 바꾸기, 배틀아이콘 띄우기, 더미 보스 끄고 진짜 보스 키기
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
