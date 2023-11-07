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
                    print(i + "번째에는 스킬이 비어있습니다");
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
        // 스킬 리스트의 자식들 중 PlayerSkill 이 있으면 추가, 아니면 null 추가
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

        //// 확인
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