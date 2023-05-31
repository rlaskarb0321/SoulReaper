using UnityEngine;


// ����1, 2, ��¡�߻� --> ȸ�Ƿ� �ڿ������� �̾����� ���ִ� StateMachineBehaviour
public class SmoothDodgeBehaviour : StateMachineBehaviour
{
    public bool _isDodgeInput;
    public float _h;
    public float _v;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isDodgeInput)
            return;

        if (Input.GetKey(KeyCode.Space))
        {
            _isDodgeInput = true;
            _h = Input.GetAxisRaw("Horizontal");
            _v = Input.GetAxisRaw("Vertical");
        }
        else
        {
            _isDodgeInput = false;
        }
    }
}
