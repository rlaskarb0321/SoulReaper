using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineChangeBGM : MonoBehaviour
{
    [Header("=== BGM Changer ===")]
    [SerializeField]
    private BGMChanger _bgmChanger;

    [SerializeField]
    private AudioClip[] _bgm;

    private enum eShrineBGM { InBGM, OutBGM, }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;
        if (!other.gameObject.tag.Equals("Player"))
            return;

        print("enter");
        _bgmChanger.ChangeBGM(_bgm[(int)eShrineBGM.InBGM]);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;
        if (!other.gameObject.tag.Equals("Player"))
            return;

        print("out");
        _bgmChanger.ChangeBGM(_bgm[(int)eShrineBGM.OutBGM]);
    }
}
