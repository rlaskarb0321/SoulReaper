using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossRoom : QuestRoom
{
    [Header("=== ���� ����� ===")]
    [SerializeField]
    private GameObject _boss;

    [SerializeField]
    private GameObject _dummyBoss;

    [SerializeField]
    private GameObject _battleICon;

    [SerializeField]
    private PlayableAsset _clearPlayable;

    [SerializeField]
    private BGMChanger _bgmChanger;

    // Field
    [Space(15.0f)]public bool _isDevelopMode;
    private PlayableDirector _playable;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
    }

    public override void SolveQuest()
    {
        // ���� �׿��� �� ȣ��, ������ ����
        _playable.playableAsset = _clearPlayable;
        _battleICon.gameObject.SetActive(false);

        StartCoroutine(_bgmChanger.FadeOutClip(_bgmChanger._originBGM, 1.0f));
        ProductionMgr.StartProduction(_playable);
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

    /// <summary>
    /// ������ �װ� ��, ���̵� ȭ����ȯ �� �ñ׳η� �������� ����� ���� ������Ʈ ���ֱ�
    /// </summary>
    /// <param name="sparrowLetter"></param>
    public void PlaceSparrowLetter(GameObject sparrowLetter)
    {
        sparrowLetter.SetActive(true);
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
