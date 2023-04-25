using UnityEngine;


// ����1, 2, ��¡�߻� --> ȸ�Ƿ� �ڿ������� �̾����� ���ִ� StateMachineBehaviour
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
