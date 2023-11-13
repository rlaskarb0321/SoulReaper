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

    // 웨이브 격퇴를 기록, 웨이브를 진행시키며 모든 웨이브가 격퇴되면 해당 방의 퀘스트 완료
    public override void SolveQuest()
    {
        // 모든 웨이브 클리어 시
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

        // 인덱스번째의 게임오브젝트를 활성화시켜서 해당 게임오브젝트 밑에있는 몬스터들을 깨운다.
        _waves[_currWave].gameObject.SetActive(true);
    }

    // 웨이브 방에 입장 시
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
