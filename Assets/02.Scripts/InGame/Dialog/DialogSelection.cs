using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IYOrNSelectOption
{
    /// <summary>
    /// ������ ��ȣ�� ��ȯ��
    /// </summary>
    /// <returns></returns>
    public int ReturnSelectResult();

    /// <summary>
    /// �������� Ư�� �ǹ̸� �ο�
    /// </summary>
    /// <param name="selectNum"></param>
    public void ApplyOption(int selectNum);
    
    /// <summary>
    /// Y �Ǵ� N �亯�� ���¿� ���� ��ü�� ��Ȳ�� ����
    /// </summary>
    /// <param name="isYes"></param>
    public void CheckAnswer(bool isYes);
}

// ��ȭ �� ������ ���� ��Ȳ�� ���̴� ��ũ��Ʈ
public class DialogSelection : MonoBehaviour
{
    [SerializeField] private Image _selectImg;
    [SerializeField] private Text _selectionText;
    [HideInInspector] public Button _btn;
    [SerializeField] public int _selectionIdx; // ������ �亯�� ����

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
