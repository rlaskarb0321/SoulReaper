using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillList : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image[] _skills;

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
        _skillList = new List<PlayerSkill>();

        // Skills 게임 오브젝트의 하위에 PlayerSkill 이 있는지 여부에 따른 작업
        for (int i = 0; i < _skills.Length; i++)
        {
            PlayerSkill skill = _skills[i].GetComponentInChildren<PlayerSkill>();

            // 스킬이 없다면, 클릭 시 스킬 UI의 바닥이 찍히도록 raycastTarget을 켜줌
            if (skill == null)
            {
                _skillList.Add(null);
                _skills[i].raycastTarget = true;
                continue;
            }

            // 스킬이 있다면, 스킬 UI 바닥의 raycast를 꺼서, 스킬이 클릭될 수 있도록 함
            _skillList.Add(skill);
            _skills[i].raycastTarget = false;
        }
    }

    /// <summary>
    /// 스킬UI에 있는 스킬들을 옮길때 쓰이는 메서드
    /// </summary>
    private void SwapSkillIndex(DummySkill grabbedSkill, PlayerSkill skillB)
    {
        Transform grabbedParent = grabbedSkill._originParent.parent;
        Transform skillBParent = skillB.transform.parent;

        grabbedSkill.SwapSkillPos(skillBParent);
        skillB.transform.SetParent(grabbedParent);
        skillB.transform.SetAsFirstSibling();
        skillB.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    // 클릭한 곳에 스킬이 있거나 비어있으면 바꾸고, 스킬 리스트가 아니면 더미를 원래 위치로 옮기기
    public void OnPointerClick(PointerEventData eventData)
    {
        // 스킬을 잡고있을 때
        if (_grabbedSkill != null)
        {
            List<RaycastResult> rayResult = new List<RaycastResult>();
            _gr.Raycast(eventData, rayResult);

            // Skill 태그가 있지않은것들을 거름
            for (int i = rayResult.Count - 1; i >= 0; i--)
            {
                if (!rayResult[i].gameObject.tag.Contains("Skill"))
                    rayResult.RemoveAt(i);
            }

            if (rayResult.Count.Equals(1))
            {
                // 1개인 경우는 잡고있는 스킬만 Cast 됐다는 뜻, 잡고있는 스킬을 놓아준다.
                _grabbedSkill.ToOriginPos();
                _grabbedSkill = null;
                return;
            }
            else
            {
                // 2개 이상인 경우는 다른 Skill 태그가 있는게 탐지되었다는 뜻, 경우의 수는 다른 스킬이거나 빈 스킬UI 이다

                // Dummy 제거
                for (int i = rayResult.Count - 1; i >= 0; i--)
                {
                    if (rayResult[i].gameObject.tag.Contains("Dummy"))
                        rayResult.RemoveAt(i);
                }

                // 다른 스킬 혹은 빈 스킬 UI만 포함됨
                for (int i = 0; i < rayResult.Count; i++)
                {
                    PlayerSkill skill = rayResult[i].gameObject.GetComponent<PlayerSkill>();
                    if (skill == null)
                    {
                        _grabbedSkill.SwapSkillPos(rayResult[i].gameObject.transform);
                    }
                    else
                    {
                        SwapSkillIndex(_grabbedSkill, skill);
                    }

                    InitSkillList();
                    _grabbedSkill.ToOriginPos();
                    _grabbedSkill = null;
                }
            }
            return;
        }

        // 스킬UI를 클릭했을때 스킬이 있는곳이면 더미가 마우스를 따라오도록 함
        DummySkill dummySkill = eventData.pointerCurrentRaycast.gameObject.GetComponent<DummySkill>();
        if (dummySkill == null)
            return;

        dummySkill.FollowMouse(this.transform);
        _grabbedSkill = dummySkill;
    }
}