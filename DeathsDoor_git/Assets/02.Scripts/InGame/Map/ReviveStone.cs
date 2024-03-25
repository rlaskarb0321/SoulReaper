using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public interface IInteractNPC
{
    public void ResetInteract();
}

public class ReviveStone : MonoBehaviour, IInteractable, IInteractNPC
{
    [Header("=== 상호 작용 ===")]
    [SerializeField]
    private Transform _interactUIFloatPos; // 상호작용 UI 키 위치

    [SerializeField]
    private string _interactName; // 상호작용 설명

    [SerializeField]
    private PlayableAsset[] _playableAssets; // 부활하고 대화하는지의 여부에 따라 다른 대화

    [Header("=== 부활 스톤 ===")]
    [SerializeField]
    private GameObject _revivePos;

    [SerializeField]
    private SkinnedMeshRenderer _dummyPlayer;

    [SerializeField]
    private Material _playerFadeMat;

    [SerializeField]
    private GameObject _reviveEffect;

    // Field
    private PlayableDirector _playableDirector;
    private bool _isInteract;
    private bool _isPlayerRevive;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    public void Revive()
    {
        CharacterData.CData cData = CharacterDataPackage._cDataInstance._characterData;
        Material fadeMat = Instantiate(_playerFadeMat);
        Color color = fadeMat.color;

        // 더미 캐릭터는 사망시에만 페이드인 시키고 그 외에는 없는듯 보이게 한다
        color.a = 0.0f;
        fadeMat.color = color;
        _dummyPlayer.material = fadeMat;

        // 캐릭터가 죽어서 씬을 새로 불러온경우
        if (cData._isPlayerDead)
        {
            // 캐릭터 데이터에 위치와 회전을 부활포지션 값으로 옮기기
            _reviveEffect.SetActive(true);
            _isPlayerRevive = true;

            cData._pos = _revivePos.transform.position;
            cData._rot = _dummyPlayer.transform.rotation;
            CharacterDataPackage._cDataInstance._characterData = cData;
            DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");

            // 맵 데이터 관리에선 꺼진 캐릭터를 받고, 더미 캐릭터를 페이드 인 시킨다.
            StartCoroutine(FadeInPlayer(fadeMat, cData));
        }
    }

    #region IInteractable Method

    public void Interact()
    {
        if (_isInteract)
            return;

        if (_isPlayerRevive)
            _playableDirector.playableAsset = _playableAssets[0];
        else
            _playableDirector.playableAsset = _playableAssets[1];

        _isInteract = true;
        _isPlayerRevive = false;
        ProductionMgr.StartProduction(_playableDirector);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_interactUIFloatPos.position);
        UIScene._instance.FloatTextUI(UIScene._instance._interactUI, value, pos, _interactName);
    }

    public void ResetInteract()
    {
        _isInteract = false;
    }

    #endregion IInteractable Method

    private IEnumerator FadeInPlayer(Material fadeMat, CharacterData.CData cData)
    {
        Color color = fadeMat.color;
        while (fadeMat.color.a < 1.0f)
        {
            color.a += Time.deltaTime * 0.5f;
            fadeMat.color = color;
            _dummyPlayer.material = fadeMat;
            yield return null;
        }

        _dummyPlayer.gameObject.SetActive(false);
        UIScene._instance._stat.gameObject.SetActive(true);
        cData._isPlayerDead = false;
        CharacterDataPackage._cDataInstance._characterData = cData;
        DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (_isInteract)
        {
            SetActiveInteractUI(false);
            return;
        }
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
    }
}
