using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillList : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject _skillsParentObj; // 지금 스킬이 있는 UI 면 Border의 Raycasttarget을 꺼야되니까 얘를 수정해야됨

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
                    print((i + 1) + "번째에는 스킬이 비어있습니다");
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
    /// 스킬UI에 있는 스킬들을 옮길때 쓰이는 메서드
    /// </summary>
    private void SwapSkillIndex()
    {

    }

    // 클릭한 곳에 스킬이 있거나 비어있으면 바꾸고, 스킬 리스트가 아니면 더미를 원래 위치로 옮기기
    public void OnPointerClick(PointerEventData eventData)
    {
        // 스킬이 있다면 Border 의 RaycastTarget을 꺼주자 없으면 켜주고

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