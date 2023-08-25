using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forge : MonoBehaviour, IInteractable
{
    private enum eDoorState { Open, Close, Count, }
    [SerializeField] private eDoorState _doorState;
    private Animator _animator;
    public bool _isPlayerEnter;
    public bool _isPlaying;

    readonly int _hashInteract = Animator.StringToHash("interact");
    readonly int _hashIsDoorOpen = Animator.StringToHash("isDoorOpen");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _animator.SetBool(_hashIsDoorOpen, _doorState == eDoorState.Open);
    }

    private void Update()
    {
        if (!_isPlayerEnter)
            return;
        if (_isPlaying)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void Interact()
    {
        int idx = (int)_doorState;
        idx++;
        idx %= (int)eDoorState.Count;

        _animator.SetTrigger(_hashInteract);
        _isPlaying = true;
        _animator.SetBool(_hashIsDoorOpen, _doorState == eDoorState.Open);
        _doorState = (eDoorState)idx;
    }

    public void SetActiveInteractUI(bool value)
    {

    }

    public void Finish()
    {
        _isPlaying = false;
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
