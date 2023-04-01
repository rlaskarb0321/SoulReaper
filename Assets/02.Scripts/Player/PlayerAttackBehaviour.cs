using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    [HideInInspector] public bool _isComboAtk;
    readonly int _hashCombo = Animator.StringToHash("AttackCombo");

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
