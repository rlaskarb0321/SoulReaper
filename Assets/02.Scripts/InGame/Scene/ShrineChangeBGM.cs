using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사당 비지엠 관리하는 클래스, 비지엠을 바꿔달라 요청하는 클래스이다.
/// </summary>
public class ShrineChangeBGM : MonoBehaviour
{
    [Header("=== BGM Changer ===")]
    [SerializeField]
    private BGMChanger _bgmChanger;

    [SerializeField]
    private AudioClip[] _bgm;

    [SerializeField]
    private float _fadeTime;

    private enum eShrineBGM { InBGM, OutBGM, }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;
        if (!other.gameObject.tag.Equals("Player"))
            return;

        _bgmChanger.ChangeDirectly(_bgm[(int)eShrineBGM.InBGM], _fadeTime);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;
        if (!other.gameObject.tag.Equals("Player"))
            return;

        _bgmChanger.ChangeDirectly(_bgmChanger._originBGM, _fadeTime);
    }
}
