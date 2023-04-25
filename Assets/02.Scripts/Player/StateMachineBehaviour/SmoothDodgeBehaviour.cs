using UnityEngine;


// 공격1, 2, 차징발사 --> 회피로 자연스럽게 이어지게 해주는 StateMachineBehaviour
public class SmoothDodgeBehaviour : StateMachineBehaviour
{
    public bool _isDodgeInput;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isDodgeInput)
            return;

        if (Input.GetKey(KeyCode.Space))
        {
            _isDodgeInput = true;
        }
        else
        {
            _isDodgeInput = false;
        }
    }
}
