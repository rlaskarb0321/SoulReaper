using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public interface IOnOffSwitchSkill
{
    /// <summary>
    /// 온, 오프 전환 가능한 스킬일 때 스킬을 온, 오프로 전환
    /// </summary>
    public void SwitchSkillActive(bool isAudioPlay);

    /// <summary>
    /// 온, 오프 스킬을 진짜로 사용하게 하는 함수
    /// </summary>
    public void UseOnOffSkill();
}

public abstract class PlayerSkill : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [Header("=== UI Inform ===")]
    [SerializeField]
    protected GameObject _uiInform;

    [HideInInspector]
    public string _skillID;

    [Header("=== Cool Down ===")]
    [SerializeField]
    protected TMP_Text _coolDownText;

    [SerializeField]
    protected Image _coolDownPanel;

    [SerializeField]
    protected float _coolDown;

    [SerializeField]
    protected float _currCoolDown;

    [Header("=== Player Character ===")]
    [SerializeField]
    protected PlayerCombat _player;
    public PlayerCombat Player { set { _player = value; } }

    protected bool _isCoolDown;

    public virtual void OnPointerEnter(PointerEventData eventData) { }

    public virtual void OnPointerExit(PointerEventData eventData) { }

    /// <summary>
    /// 스킬을 사용할 때 호출되는 함수
    /// </summary>
    public abstract void UseSkill();
}
