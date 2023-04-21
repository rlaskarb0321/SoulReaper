using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEndBehaviour : StateMachineBehaviour
{
    public bool _isEnd;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isEnd = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isEnd = true;
    }
}
