using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HealthSeed : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;
    [SerializeField] private GameObject _mesh;

    private PlayerData _player;
    private bool _isInteract;
    private PlayableDirector _playable;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
    }

    public void Interact()
    {
        if (!CharacterDataPackage._cDataInstance._characterData._alreadySeedGet)
            ProductionMgr.StartProduction(_playable);

        UIScene._instance.UpdateSeedCount(CharacterDataPackage._cDataInstance._characterData._seedCount + 1);
        UIScene._instance._seedUI.GoDownSeedUI();
        SetActiveInteractUI(false);
        GetComponent<SphereCollider>().enabled = false;

        _isInteract = true;
        _mesh.SetActive(false);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, _interactName);
    }

    #region Trigger Interact Method
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_player == null)
            _player = other.GetComponent<PlayerData>();

        UIScene._instance._seedUI.PopUpSeedUI();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (Input.GetKey(KeyCode.F) && !_isInteract)
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
        UIScene._instance._seedUI.GoDownSeedUI();
    }
    #endregion Trigger Interact Method
}
