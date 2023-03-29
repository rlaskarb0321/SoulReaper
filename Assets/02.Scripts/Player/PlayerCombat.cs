using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Transform _player;
    private Camera _cam;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _cam = Camera.main;
        _player = transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // �ٰŸ� ���� 1����
            RotateToClickDir();
        }
        else if (Input.GetMouseButton(1))
        {
            // ���Ÿ�����
            RotateToClickDir();
        }
    }

    void RotateToClickDir()
    {
        RaycastHit hit;
        Vector3 clickVector;
        Vector3 dir;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit))
        {
            clickVector = new Vector3(hit.point.x, _player.position.y, hit.point.z);
            dir = clickVector - _player.position;
            transform.forward = dir;
        }
    }
}
