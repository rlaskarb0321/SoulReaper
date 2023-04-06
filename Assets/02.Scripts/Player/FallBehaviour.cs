using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBehaviour : StateMachineBehaviour
{
    public bool _isFall;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isFall = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isFall = false;
    }
}
