using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPlant : MonoBehaviour, IInteractable
{
    public enum eFlowerState { None, Growing, Bloom, harvested, }

    [Header("=== Interact ===")]
    [SerializeField] private string[] _interactName;
    [SerializeField] private string _notEnoughtSeed;
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
                _apply.EditData();
            }
        }
    }

    [SerializeField]
    private float _harvestDelay;

    [Header("=== Data ===")]
    [SerializeField] private DataApply _apply;

    // field
    private readonly int _hashSeed = Animator.StringToHash("Seed");
    private readonly int _hashEat = Animator.StringToHash("Eat");
    private AudioSource _audio;
    private Animator _animator;
    private PlayerData _player;
    private BoxCollider _interactColl;
    private float _originHarvestDelay;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _interactColl = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _originHarvestDelay = _harvestDelay;
    }

    private void Update()
    {
        if (FlowerState == eFlowerState.harvested)
            DelayPlant();
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

        string interactName = "";
        switch (FlowerState)
        {
            case eFlowerState.None:
                interactName = _interactName[(int)eFlowerState.None];
                break;

            case eFlowerState.Bloom:
                interactName = _interactName[(int)eFlowerState.Bloom];
                break;
        }
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, interactName);
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

    /// <summary>
    /// ��Ȯ�� ������ �� ��õ��� ���� ���ϵ��� �����̸� ��
    /// </summary>
    private void DelayPlant()
    {
        if (_harvestDelay <= 0.0f)
        {
            FlowerState = eFlowerState.None;
            _harvestDelay = _originHarvestDelay;
            _interactColl.enabled = true;
            return;
        }

        SetActiveInteractUI(false);
        _interactColl.enabled = false;
        _harvestDelay -= Time.deltaTime;
    }

    // ��ȣ�ۿ����� ���� ������ ����ϴ� �޼���
    private void PlantHealthSeed()
    {
        if (_player._seedCount <= 0)
        {
            UIScene._instance.UpdateSeedCount(0);
            UIScene._instance.FloatAlarmTextUI(_notEnoughtSeed);
            return;
        }

        UIScene._instance.UpdateSeedCount(CharacterDataPackage._cDataInstance._characterData._seedCount - 1);
        _animator.SetTrigger(_hashSeed);
        FlowerState = eFlowerState.Growing;
    }

    // ���������� ���� ���¸� �ٲٴ� �۾�
    public void GrownPlant() => _animator.enabled = true;

    // ��ȣ�ۿ����� ���� ���Ÿ� ������ ����ϴ� �޼ҵ�
    private void EatFlower()
    {
        HarvestPlant();

        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.HP, _player._maxHP, _player._maxHP);
        UIScene._instance._seedUI.GoDownSeedUI();
        _audio.PlayOneShot(_audio.clip, _audio.volume * SettingData._sfxVolume);
    }

    // ���������� ���� ���¸� �ٲٴ� �۾�
    public void HarvestPlant()
    {
        FlowerState = eFlowerState.harvested;
        _animator.SetTrigger(_hashEat);
    }

    // ���� ���� ����Ǿ��� �� animator�� ������Ű��
    public void EndGrown()
    {
        FlowerState = eFlowerState.Bloom;
    }
    #endregion Health Plant Method
}
