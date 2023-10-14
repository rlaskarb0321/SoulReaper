using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IYOrNSelectOption
{
    /// <summary>
    /// 선택한 번호를 반환함
    /// </summary>
    /// <returns></returns>
    public int ReturnSelectResult();

    /// <summary>
    /// 선택지에 특정 의미를 부여
    /// </summary>
    /// <param name="selectNum"></param>
    public void ApplyOption(int selectNum);
    
    /// <summary>
    /// Y 또는 N 답변의 상태에 따라 객체가 상황을 조정
    /// </summary>
    /// <param name="isYes"></param>
    public void CheckAnswer(bool isYes);
}

// 대화 중 선택지 고르는 상황에 쓰이는 스크립트
public class DialogSelection : MonoBehaviour
{
    [SerializeField] private Image _selectImg;
    [SerializeField] private Text _selectionText;
    [HideInInspector] public Button _btn;
    [SerializeField] public int _selectionIdx; // 선택지 답변을 저장

    public enum eYesOrNo { Yes, No, };

    private void Awake()
    {
        _btn = GetComponent<Button>();
        SelectSelection(false);
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
