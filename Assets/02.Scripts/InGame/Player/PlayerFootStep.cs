using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    [SerializeField] private PlayerFSM _state;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_state.State != PlayerFSM.eState.Move)
        {
            _audio.Stop();
            return;
        }

        if (_state.State == PlayerFSM.eState.Move)
        {
            if (!_audio.isPlaying)
            {
                _audio.Play();
            }
        }
    }
}
