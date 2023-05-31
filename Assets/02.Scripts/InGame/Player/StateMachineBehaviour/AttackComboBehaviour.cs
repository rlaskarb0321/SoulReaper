using UnityEngine;

public class AttackComboBehaviour : StateMachineBehaviour
{
    public bool _isComboAtk;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isComboAtk)
            return;

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
