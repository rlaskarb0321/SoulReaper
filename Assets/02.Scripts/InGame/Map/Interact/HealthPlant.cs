using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (_flowerState != eFlowerState.Growing)
            {

            }
        }
    }
    [SerializeField]
    private GameObject _healthFlower;
    [SerializeField]
    private GameObject[] _leaves;

    [Header("=== Data ===")]
    [SerializeField] private LittleForestData _map;

    // field
    private AudioSource _audio;
    private Animator _animator;
    private PlayerData _player;
    private string _sceneName;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        // 현재 열려있는 씬이 어느 씬인지 판단해야 꽃의 상태가 바뀌었을 때 어떤 맵 데이터 객체를 만들지 결정할 수 있음
        int index = UnityEngine.SceneManagement.SceneManager.sceneCount;
        for (int i = 0; i < index; i++)
        {
            print(UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name);
        }
    }

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

    private void PlantHealthSeed()
    {
        _animator.enabled = true;
        FlowerState = eFlowerState.Growing;
    }

    private void EatFlower()
    {
        UIScene._instance.UpdateHPMP(UIScene.ePercentageStat.Hp, _player._maxHP, _player._maxHP);
        UIScene._instance._seedUI.GoDownSeedUI();
        _audio.PlayOneShot(_audio.clip);
        FlowerState = eFlowerState.harvested;

        for (int i = 0; i < _leaves.Length; i++)
        {
            _leaves[i].SetActive(false);
        }
    }

    public void EndGrown()
    {
        FlowerState = eFlowerState.Bloom;
        _animator.enabled = false;
    }

    public void SetActiveInteractUI(bool value)
    {
        if (FlowerState == eFlowerState.harvested || FlowerState == eFlowerState.Growing)
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
}
