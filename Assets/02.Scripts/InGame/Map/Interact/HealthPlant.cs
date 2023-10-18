using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlant : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    private enum eFlowerState { None, Growing, Bloom, harvested, }
    [Header("=== Plant ===")]
    [SerializeField] private eFlowerState _flowerState;
    [SerializeField] private GameObject _healthFlower;
    [SerializeField] private GameObject[] _leaves;

    private AudioSource _audio;
    private Animator _animator;
    private PlayerData _player;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (_flowerState == eFlowerState.harvested || _flowerState == eFlowerState.Growing)
            return;

        switch (_flowerState)
        {
            case eFlowerState.None:
                PlantHealthSeed();
                break;
            case eFlowerState.Bloom:
                EatPlant();
                break;
        }
    }

    private void PlantHealthSeed()
    {
        _animator.enabled = true;
        _flowerState = eFlowerState.Growing;
    }

    private void EatPlant()
    {
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Hp, _player._maxHP, _player._maxHP);
        UIScene._instance._seedUI.GoDownSeedUI();
        _audio.PlayOneShot(_audio.clip);
        _flowerState = eFlowerState.harvested;

        for (int i = 0; i < _leaves.Length; i++)
        {
            _leaves[i].SetActive(false);
        }
    }

    public void EndGrown()
    {
        _flowerState = eFlowerState.Bloom;
        _animator.enabled = false;
    }

    public void SetActiveInteractUI(bool value)
    {
        if (_flowerState == eFlowerState.harvested || _flowerState == eFlowerState.Growing)
            value = false;

        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_player == null)
            _player = other.GetComponent<PlayerData>();
        if (_flowerState != eFlowerState.harvested)
            UIScene._instance._seedUI.PopUpSeedUI();
    }

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

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
        UIScene._instance._seedUI.GoDownSeedUI();
    }
}
