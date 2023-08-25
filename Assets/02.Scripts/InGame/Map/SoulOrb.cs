using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulOrb : MonoBehaviour, IInteractable
{
    private bool _isPlayerEnter;

    private void Update()
    {
        if (!_isPlayerEnter)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void Interact()
    {
        print("soul + 100");
        Destroy(gameObject);
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        _isPlayerEnter = true;
        SetActiveInteractUI(_isPlayerEnter);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        _isPlayerEnter = false;
        SetActiveInteractUI(_isPlayerEnter);
    }
}