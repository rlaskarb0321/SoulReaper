using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IYOrNSelectOption
{
    public int ReturnSelectResult();

    public void ApplyOption(int selectNum);

    public void EndDialog();
}

// 대화 중 선택지 고르는 상황에 쓰이는 스크립트
public class DialogSelection : MonoBehaviour
{
    [SerializeField] private Image _selectImg;
    [SerializeField] private Text _selectionText;
    [HideInInspector] public Button _btn;
    public int _selectionIdx; // 선택지 답변을 저장

    public int SelectionNum { get { return _selectionIdx; } }

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void InputSelectionData(string dialog)
    {
        _selectionText.text = dialog;
    }

    public void RemoveAllListenerSelection() => _btn.onClick.RemoveAllListeners();

    public void AddListenerOnClick(UnityEngine.Events.UnityAction call) => _btn.onClick.AddListener(call);

    // 선택지 마우스 오버 & 마우스 탈출 등 ux 관련 메서드
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
