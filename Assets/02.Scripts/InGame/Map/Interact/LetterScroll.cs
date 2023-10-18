using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterScroll : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    [SerializeField] private GameObject _letterMesh;
    [SerializeField] private CarrierPigeon _carrierPigeon;

    public void Interact()
    {
        if (_carrierPigeon != null)
        {
            _carrierPigeon.FlyAway();
        }

        _letterMesh.SetActive(false);
        SetActiveInteractUI(false);
        UIScene._instance.SetUIPanelActive(UIScene._instance._letterPanel);
        GetComponent<SphereCollider>().enabled = false;
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }

    // ��ȣ�ۿ� UIǥ��, F Ű ������ ��ȣ�ۿ�
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }
        
        SetActiveInteractUI(true);
    }
}
