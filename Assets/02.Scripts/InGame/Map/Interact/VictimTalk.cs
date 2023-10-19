using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictimTalk : MonoBehaviour
{
    private Victim _victimBody;

    private void Awake()
    {
        _victimBody = GetComponentInParent<Victim>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        print("Player In");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        print("Player Out");
    }
}
