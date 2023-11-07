using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillList : MonoBehaviour
{
    [SerializeField]
    private GameObject _skillsParentObj;

    private KeyCode[] _keyCode = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    private List<PlayerSkill> _skillList;

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
        for (int i = 0; i < _keyCode.Length; i++)
        {
            if (Input.GetKeyDown(_keyCode[i]))
            {
                if (_skillList[i] == null)
                {
                    print(i + "��°���� ��ų�� ����ֽ��ϴ�");
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
            PlayerSkill skill = _skillsParentObj.transform.GetChild(i).GetComponent<PlayerSkill>();
            if (skill == null)
            {
                _skillList.Add(null);
            }
            else
            {
                _skillList.Add(skill);
            }
        }

        //// Ȯ��
        //for (int i = 0; i < _skillList.Count; i++)
        //{
        //    if(_skillList[i] == null)
        //    {
        //        print(i + " null");
        //    }
        //    else
        //    {
        //        print(i + " " + _skillList[i].GetType());
        //    }
        //}
    }
}