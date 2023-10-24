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
        if (Input.GetKey(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }
}
