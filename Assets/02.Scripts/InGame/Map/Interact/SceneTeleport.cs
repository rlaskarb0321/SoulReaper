using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTeleport : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField] private string _interactName;
    [SerializeField] private Transform _floatUIPos;

    [Header("=== Scene ===")]
    [SerializeField] private string _nextMap;
    [SerializeField] private Animator _sceneOutPanel;

    private Image _sceneOutPanelImg;

    private void Awake()
    {
        _sceneOutPanelImg = _sceneOutPanel.gameObject.GetComponent<Image>();
    }

    public void Interact()
    {
        _sceneOutPanel.gameObject.SetActive(true);
        _sceneOutPanel.enabled = true;

        // 여기서 CharacterDataPackage._characterData 의 값을 가져와서 hp, mp 등을 유지하고
        // 다음 씬의 이름에따라 플레이어의 위치와 회전값만 바꿔주고, CharacterDataPackage._characterData 를 갱신시키자
        CharacterData.CData data = CharacterDataPackage._characterData._characterData;
        switch (_nextMap)
        {
            case "Castle_Map":
                data._pos = new Vector3(ConstData.FROM_FOREST_TO_CASTLE_POS_X, ConstData.FROM_FOREST_TO_CASTLE_POS_Y, ConstData.FROM_FOREST_TO_CASTLE_POS_Z);
                data._rot = new Quaternion(ConstData.FROM_FOREST_TO_CASTLE_ROT_X, ConstData.FROM_FOREST_TO_CASTLE_ROT_Y, ConstData.FROM_FOREST_TO_CASTLE_ROT_Z, ConstData.FROM_FOREST_TO_CASTLE_ROT_W);
                break;

            case "LittleForest_Map":
                data._pos = new Vector3(ConstData.FROM_CASTLE_TO_FOREST_POS_X, ConstData.FROM_CASTLE_TO_FOREST_POS_Y, ConstData.FROM_CASTLE_TO_FOREST_POS_Z);
                data._rot = new Quaternion(ConstData.FROM_CASTLE_TO_FOREST_ROT_X, ConstData.FROM_CASTLE_TO_FOREST_ROT_Y, ConstData.FROM_CASTLE_TO_FOREST_ROT_Z, ConstData.FROM_CASTLE_TO_FOREST_ROT_W);
                break;
        }

        CharacterDataPackage._characterData._characterData = data;
        DataManage.SaveCData(CharacterDataPackage._characterData, "TestCData");
        StartCoroutine(TeleportScene());
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    private IEnumerator TeleportScene()
    {
        while (_sceneOutPanelImg.color.a < 1.0f)
        {
            if (_sceneOutPanelImg.color.a >= 1.0f)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        LoadingScene.LoadScene(_nextMap);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }
}
