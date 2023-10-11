using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��ȭ �� ������ ���� ��Ȳ�� ���̴� ��ũ��Ʈ
public class DialogSelection : MonoBehaviour
{
    [SerializeField] private Image _selectImg;
    [SerializeField] private Text _selectionText;
    [HideInInspector] public Button _btn;
    public bool _isYes;

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void InputSelectionData(string dialog)
    {
        _selectionText.text = dialog;
    }

    public void RemoveAllListenerSelection() => _btn.onClick.RemoveAllListeners();

    public void AddListenerSelection(UnityEngine.Events.UnityAction call) => _btn.onClick.AddListener(call);

    // ������ ���콺 ���� & ���콺 Ż�� �� ux ���� �޼���
    public void SelectSelection(bool isSelect)
    {
        _selectImg.gameObject.SetActive(isSelect);

        if (isSelect)
        {

        }
        else
        {

        }
    }
}
