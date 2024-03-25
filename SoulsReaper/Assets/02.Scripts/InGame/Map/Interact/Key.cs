using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    [Header("=== Key ===")]
    public Transform _keyObj;
    public float _rotSpeed;

    private SphereCollider _sphereColl;
    private Animator _animator;
    private Rigidbody _rbody;
    private RigidbodyConstraints _previousConstraints;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _apply;

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

    public void Reward()
    {
        _rbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        _rbody.constraints = _previousConstraints;
        _sphereColl.enabled = true;
        _animator.enabled = true;
    }

    public void Interact()
    {
        if (!_keyObj.gameObject.activeSelf)
            return;

        SetActiveInteractUI(false);
        _keyObj.gameObject.SetActive(false);
        _apply.EditData();
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, _interactName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        if (Input.GetKey(KeyCode.F))
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
