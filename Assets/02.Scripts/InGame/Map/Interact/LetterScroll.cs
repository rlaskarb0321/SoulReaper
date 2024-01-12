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
    [SerializeField] private TextAsset _letterContent;

    [Header("=== Data ===")]
    [SerializeField] private ForestDataApply _apply;

    public void Interact()
    {
        DialogMgr dialogMgr = new DialogMgr();
        if (_carrierPigeon != null)
        {
            _carrierPigeon.FlyAway();
        }

        _letterMesh.SetActive(false);
        SetActiveInteractUI(false);
        UIScene._instance._letter.SetText(dialogMgr.ParsingCSVLine(_letterContent));
        UIScene._instance.SetUIPanelActive(UIScene._instance._letter.gameObject);
        GetComponent<SphereCollider>().enabled = false;

        // ���⼭ ������ ����
        _apply.EditData();
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, _interactName);
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
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
            return;
        }
        
        SetActiveInteractUI(true);
    }
}
