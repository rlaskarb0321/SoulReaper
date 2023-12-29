using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _activateSound;

    private enum eActivate { SetOff, SetOn }

    public virtual void PlaySetActiveSound(bool isTurnOn, AudioSource audio)
    {
        if (isTurnOn)
        {
            audio.PlayOneShot(_activateSound[(int)eActivate.SetOn]);
        }
        else
        {
            audio.PlayOneShot(_activateSound[(int)eActivate.SetOff]);
        }
    }
}