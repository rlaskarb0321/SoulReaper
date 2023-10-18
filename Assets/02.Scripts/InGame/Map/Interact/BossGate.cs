using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    public GameObject[] _seals;

    public void Interact()
    {

    }

    public void SetActiveInteractUI(bool value)
    {

    }
}
