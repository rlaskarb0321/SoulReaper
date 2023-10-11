using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSelection : MonoBehaviour
{
    [SerializeField] private Image _selectImg;
    [SerializeField] private Text _selectionText;
    [HideInInspector] public Button _btn;

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
