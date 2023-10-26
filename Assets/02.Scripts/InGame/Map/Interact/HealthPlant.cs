using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPlant : MonoBehaviour, IInteractable
{
    public enum eFlowerState { None, Growing, Bloom, harvested, }

    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;
    
    [Header("=== Plant ===")]
    private eFlowerState _flowerState;
    public eFlowerState FlowerState
    {   
        get { return _flowerState; }
        set 
        { 
            _flowerState = value;
            if (_flowerState != eFlowerState.Growing)
            {
                _apply.EditMapData();
            }
        }
    }
    [SerializeField]
    private GameObject _healthFlower;
    [SerializeField]
    private GameObject[] _leaves;

    [Header("=== Data ===")]
    [SerializeField] private DataApply _apply;

    // field
    private AudioSource _audio;
    private Animator _animator;
    private PlayerData _player;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    #region Interface IInteractable Method
    public void Interact()
    {
        if (FlowerState == eFlowerState.harvested || FlowerState == eFlowerState.Growing)
            return;

        switch (FlowerState)
        {
            case eFlowerState.None:
                PlantHealthSeed();
                break;
            case eFlowerState.Bloom:
                EatFlower();
                break;
        }
    }

    public void SetActiveInteractUI(bool value)
    {
        if (FlowerState == eFlowerState.harvested || FlowerState == eFlowerState.Growing)
            value = false;

        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }
    #endregion Interface IInteractable Method

    #region Trigger Interact Method
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_player == null)
            _player = other.GetComponent<PlayerData>();
        if (FlowerState != eFlowerState.harvested)
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
    #endregion Trigger Interact Method

    #region Health Plant Method
    // 상호작용으로 꽃을 심을때 사용하는 메서드
    private void PlantHealthSeed()
    {
        GrownPlant();
        FlowerState = eFlowerState.Growing;
    }

    // 직접적으로 꽃의 상태를 바꾸는 작업
    public void GrownPlant() => _animator.enabled = true;

    // 상호작용으로 꽃의 열매를 먹을때 사용하는 메소드
    private void EatFlower()
    {
        HarvestPlant();

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Hp, _player._maxHP, _player._maxHP);
        UIScene._instance._seedUI.GoDownSeedUI();
        _audio.PlayOneShot(_audio.clip);
    }

    // 직접적으로 꽃의 상태를 바꾸는 작업
    public void HarvestPlant()
    {
        FlowerState = eFlowerState.harvested;

        for (int i = 0; i < _leaves.Length; i++)
        {
            _leaves[i].SetActive(false);
        }
    }

    // 꽃이 완전 성장되었을 때 animator를 정지시키기
    public void EndGrown()
    {
        FlowerState = eFlowerState.Bloom;
        _animator.enabled = false;
    }
    #endregion Health Plant Method
}
