using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    private Ladder _ladder;
    [SerializeField] private Ladder.eTriggerPos _myPos;

    private void Awake()
    {
        _ladder = transform.parent.GetComponent<Ladder>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _ladder._triggerPos = _myPos;
    }
}
