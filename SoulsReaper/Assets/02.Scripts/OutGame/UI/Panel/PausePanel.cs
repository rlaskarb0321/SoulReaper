using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : UIPanel
{
    public override void PlaySetActiveSound(bool isTurnOn, AudioSource audio)
    {
        base.PlaySetActiveSound(isTurnOn, audio);
    }
}
