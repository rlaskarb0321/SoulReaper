using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    [Header("=== Reference State ===")]
    [SerializeField]
    private PlayerFSM _state;

    [SerializeField]
    private PlayerMove_1 _playerMove;

    [Header("=== Sound Clip ===")]
    [SerializeField]
    private AudioClip[] _sounds;

    // Field
    private enum eAudio { Sprint, Ladder, }
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_state.State != PlayerFSM.eState.Move && _state.State != PlayerFSM.eState.Ladder)
        {
            _audio.Stop();
            return;
        }

        switch (_state.State)
        {
            case PlayerFSM.eState.Move:
                if (!_audio.isPlaying)
                {
                    _audio.clip = _sounds[(int)eAudio.Sprint];
                    _audio.Play();
                }
                break;

            case PlayerFSM.eState.Ladder:
                if (!_playerMove._animator.GetBool(_playerMove._hashLadderInput))
                {
                    _audio.Stop();
                    return;
                }

                if (!_audio.isPlaying)
                {
                    _audio.clip = _sounds[(int)eAudio.Ladder];
                    _audio.Play();
                }
                break;
        }

        //if (_state.State == PlayerFSM.eState.Move)
        //{
        //    if (!_audio.isPlaying)
        //    {
        //        _audio.clip = _sounds[(int)eAudio.Sprint];
        //        _audio.Play();
        //    }
        //}
    }
}
