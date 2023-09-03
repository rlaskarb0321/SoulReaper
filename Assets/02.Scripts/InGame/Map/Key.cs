using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public Transform _keyObj;
    public float _rotSpeed;
    private SphereCollider _sphereColl;

    private void Awake()
    {
        _sphereColl = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        if (!_keyObj.gameObject.activeSelf)
            return;

        _keyObj.Rotate(Vector3.up * _rotSpeed * Time.deltaTime);
    }

    public void SolveReward()
    {
        _sphereColl.enabled = true;
        print("solve");
    }

    public void Interact()
    {
        SetActiveInteractUI(false);
        print("¿­¼è½Àµæ");
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
