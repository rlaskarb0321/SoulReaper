using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaidRoom : QuestRoom
{
    [Header("=== Alarm ===")]
    public GameObject _entranceBlockObj;
    public GameObject _ladder;

    [Header("=== Raid Room ===")]
    public RaidWave[] _waves;
    public int _currWave = 0;

    [Header("=== BGM Changer ===")]
    [SerializeField]
    private BGMChanger _bgmChanger;

    [SerializeField]
    private AudioClip[] _bgm;

    [SerializeField]
    private float _fadeTime;

    [Header("=== apply ===")]
    public DataApply _apply;

    private enum eCombatBGM { Common, Combat, }

    // ���̺� ���� ���, ���̺긦 �����Ű�� ��� ���̺갡 ����Ǹ� �ش� ���� ����Ʈ �Ϸ�
    public override void SolveQuest()
    {
        // ��� ���̺� Ŭ���� ��
        if (_currWave >= _waves.Length)
        {
            _ladder.gameObject.SetActive(true);
            _entranceBlockObj.SetActive(false);
            StartCoroutine(_bgmChanger.FadeOutClip(_bgm[(int)eCombatBGM.Common], _fadeTime));

            if (_apply != null)
            {
                _apply.EditData();
            }
            return;
        }

        // �ε�����°�� ���ӿ�����Ʈ�� Ȱ��ȭ���Ѽ� �ش� ���ӿ�����Ʈ �ؿ��ִ� ���͵��� �����.
        _waves[_currWave].gameObject.SetActive(true);
    }

    // ���̺� �濡 ���� ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
        {
            return;
        }

        // _bgmChanger.ChangeDirectly(_bgm[(int)eCombatBGM.Combat], _fadeTime);
        _entranceBlockObj.SetActive(true);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        SolveQuest();
        StartCoroutine(_bgmChanger.FadeOutClip(_bgm[(int)eCombatBGM.Combat], _fadeTime));
    }
}
