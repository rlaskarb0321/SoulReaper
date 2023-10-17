using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlant : MonoBehaviour, IInteractable
{
    private enum eFlowerState { None, Growing, Bloom, harvested, }

    [SerializeField] private eFlowerState _flowerState;
    [SerializeField] private GameObject _healthFlower;
    [SerializeField] private GameObject[] _leaves;

    private AudioSource _audio;
    private Animator _animator;
    private UIScene _ui;
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
        print("Heal!");
        _audio.PlayOneShot(_audio.clip);
        _ui.UpdateHPMP(UIScene.ePercentageStat.Hp, _player._maxHP, _player._maxHP);
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
            return;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_ui == null)
            _ui = other.GetComponent<PlayerData>()._ui;
        if (_player == null)
            _player = other.GetComponent<PlayerData>();

        _ui._seedUI.PopUpSeedUI();
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
        _ui._seedUI.GoDownSeedUI();
        print("out");
    }
}
