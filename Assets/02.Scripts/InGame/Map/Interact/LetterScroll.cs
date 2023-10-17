using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterScroll : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _letterUI;
    [SerializeField] private GameObject _letterMesh;
    [SerializeField] private CarrierPigeon _carrierPigeon;

    private UIScene _ui;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _letterUI.SetActive(false);
        }
    }

    public void Interact()
    {
        if (_carrierPigeon != null)
        {
            _carrierPigeon.FlyAway();
        }

        _letterMesh.SetActive(false);
        _ui.SetUIPanelActive(_ui._letterPanel);
        GetComponent<SphereCollider>().enabled = false;
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_ui == null)
            _ui = other.GetComponent<PlayerData>()._ui;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }

    // 상호작용 UI표시, F 키 누르면 상호작용
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
