using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioListener : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    void Update()
    {
        transform.position = _player.transform.position;
    }
}
