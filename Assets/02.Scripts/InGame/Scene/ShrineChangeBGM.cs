using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ������ �����ϴ� Ŭ����, �������� �ٲ�޶� ��û�ϴ� Ŭ�����̴�.
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
