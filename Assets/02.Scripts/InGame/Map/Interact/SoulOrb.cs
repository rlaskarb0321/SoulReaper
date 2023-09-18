using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrb : MonoBehaviour, IInteractable
{
    private PlayerStat _playerStat;
    private bool _isTriggered;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (!_isTriggered || _playerStat == null)
            return;

        print("Soul Orb + 100");
    }

    public void SetActiveInteractUI(bool value)
    {
        // 상호작용 UI 키기 끄기
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(true);
        _isTriggered = true;
        _playerStat = other.GetComponent<PlayerStat>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
        _isTriggered = false;
    }
}