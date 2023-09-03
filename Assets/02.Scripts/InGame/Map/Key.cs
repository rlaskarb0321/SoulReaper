using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public Transform _keyObj;
    public float _rotSpeed;

    private SphereCollider _sphereColl;
    private Animator _animator;
    private Rigidbody _rbody;
    private RigidbodyConstraints _previousConstraints;

    private void Awake()
    {
        _sphereColl = _keyObj.GetComponent<SphereCollider>();
        _animator = GetComponent<Animator>();
        _rbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _previousConstraints = _rbody.constraints;
    }

    private void Update()
    {
        if (!_keyObj.gameObject.activeSelf)
            return;

        _keyObj.Rotate(Vector3.up * _rotSpeed * Time.deltaTime);
    }

    public void SolveReward()
    {
        _rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        _rbody.constraints = _previousConstraints;
        _sphereColl.enabled = true;
        _animator.enabled = true;
        print("solve");
    }

    public void Interact()
    {
        SetActiveInteractUI(false);
        print("�������");
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }
}
