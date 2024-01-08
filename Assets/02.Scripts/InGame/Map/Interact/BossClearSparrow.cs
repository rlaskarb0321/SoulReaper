using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossClearSparrow : MonoBehaviour, IInteractable
{
    [Header("=== 상호 작용 ===")]
    [SerializeField]
    private Transform _interactUIFloatPos; // 상호작용 UI 키 위치

    [SerializeField]
    private string _interactName; // 상호작용 설명

    [SerializeField]
    private GameObject _letterMesh;

    [SerializeField]
    private TextAsset _letterContent;

    // Field
    private PlayableDirector _playable;

    private void Awake()
    {
        _playable = GetComponent<PlayableDirector>();
    }

    public void Interact()
    {
        DialogMgr dialogMgr = new DialogMgr();

        _letterMesh.SetActive(false);
        SetActiveInteractUI(false);
        UIScene._instance._letter.SetText(dialogMgr.ParsingCSVLine(_letterContent));
        UIScene._instance.SetUIPanelActive(UIScene._instance._letter.gameObject);
        GetComponent<SphereCollider>().enabled = false;

        // 데모 버전용 게임오버 띄우기
        StartCoroutine(ClearDemoVersion());
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_interactUIFloatPos.position);
        UIScene._instance.FloatTextUI(UIScene._instance._interactUI, value, pos, _interactName);
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
    }

    private IEnumerator ClearDemoVersion()
    {
        yield return new WaitUntil(() => UIScene._instance._letter.gameObject.activeSelf == false);

        ProductionMgr.StartProduction(_playable);
    }
}
