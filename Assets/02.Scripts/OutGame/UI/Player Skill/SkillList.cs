using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillList : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject _skillsParentObj; // ���� ��ų�� �ִ� UI �� Border�� Raycasttarget�� ���ߵǴϱ� �긦 �����ؾߵ�

    [SerializeField]
    private GraphicRaycaster _gr;

    private KeyCode[] _keyCode = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    private List<PlayerSkill> _skillList;
    private DummySkill _grabbedSkill;

    private void Awake()
    {
        InitSkillList();
    }

    private void Update()
    {
        UseSkillInTheSkillList();
    }

    private void UseSkillInTheSkillList()
    {
        if (!Input.anyKeyDown)
            return;

        for (int i = 0; i < _keyCode.Length; i++)
        {
            if (Input.GetKeyDown(_keyCode[i]))
            {
                if (_skillList[i] == null)
                {
                    print((i + 1) + "��°���� ��ų�� ����ֽ��ϴ�");
                    return;
                }

                _skillList[i].UseSkill();
            }
        }
    }

    private void InitSkillList()
    {
        PlayerSkill[] skills = _skillsParentObj.GetComponentsInChildren<PlayerSkill>();
        
        _skillList = new List<PlayerSkill>();
        // ��ų ����Ʈ�� �ڽĵ� �� PlayerSkill �� ������ �߰�, �ƴϸ� null �߰�
        for (int i = 0; i < _skillsParentObj.transform.childCount; i++)
        {
            PlayerSkill skill = _skillsParentObj.transform.GetChild(i).GetComponentInChildren<PlayerSkill>();
            if (skill == null)
            {
                _skillList.Add(null);
            }
            else
            {
                _skillList.Add(skill);
            }
        }
    }

    /// <summary>
    /// ��ųUI�� �ִ� ��ų���� �ű涧 ���̴� �޼���
    /// </summary>
    private void SwapSkillIndex()
    {

    }

    // Ŭ���� ���� ��ų�� �ְų� ��������� �ٲٰ�, ��ų ����Ʈ�� �ƴϸ� ���̸� ���� ��ġ�� �ű��
    public void OnPointerClick(PointerEventData eventData)
    {
        // ��ų�� �ִٸ� Border �� RaycastTarget�� ������ ������ ���ְ�

        if (_grabbedSkill != null)
        {
            List<RaycastResult> list = new List<RaycastResult>();
            _gr.Raycast(eventData, list);
            for (int i = 0; i < list.Count; i++)
            {
                print(list[i].gameObject.name);
            }
            _grabbedSkill.ToOriginPos();
            _grabbedSkill = null;
            return;
        }

        print("isnt grab " + eventData.pointerCurrentRaycast.gameObject.name);
        DummySkill dummySkill = eventData.pointerCurrentRaycast.gameObject.GetComponent<DummySkill>();
        if (dummySkill == null)
            return;

        dummySkill.FollowMouse(this.transform);
        _grabbedSkill = dummySkill;
    }
}