using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    [HideInInspector] public bool _isComboAtk;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isComboAtk = true;
        }
        else
        {
            _isComboAtk = false;
        }
    }
}
