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
    [Header("=== ��ȣ �ۿ� ===")]
    [SerializeField]
    private Transform _interactUIFloatPos; // ��ȣ�ۿ� UI Ű ��ġ

    [SerializeField]
    private string _interactName; // ��ȣ�ۿ� ����

    [SerializeField]
    private PlayableAsset[] _playableAssets; // ��Ȱ�ϰ� ��ȭ�ϴ����� ���ο� ���� �ٸ� ��ȭ

    [Header("=== ��Ȱ ���� ===")]
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

        // ���� ĳ���ʹ� ����ÿ��� ���̵��� ��Ű�� �� �ܿ��� ���µ� ���̰� �Ѵ�
        color.a = 0.0f;
        fadeMat.color = color;
        _dummyPlayer.material = fadeMat;

        // ĳ���Ͱ� �׾ ���� ���� �ҷ��°��
        if (cData._isPlayerDead)
        {
            // ĳ���� �����Ϳ� ��ġ�� ȸ���� ��Ȱ������ ������ �ű��
            _reviveEffect.SetActive(true);
            _isPlayerRevive = true;

            cData._pos = _revivePos.transform.position;
            cData._rot = _dummyPlayer.transform.rotation;
            CharacterDataPackage._cDataInstance._characterData = cData;
            DataManage.SaveCData(CharacterDataPackage._cDataInstance, "TestCData");

            // �� ������ �������� ���� ĳ���͸� �ް�, ���� ĳ���͸� ���̵� �� ��Ų��.
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
