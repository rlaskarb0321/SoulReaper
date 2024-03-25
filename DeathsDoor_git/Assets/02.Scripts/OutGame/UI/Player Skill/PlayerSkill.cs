using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public interface IOnOffSwitchSkill
{
    /// <summary>
    /// ��, ���� ��ȯ ������ ��ų�� �� ��ų�� ��, ������ ��ȯ
    /// </summary>
    public void SwitchSkillActive(bool isAudioPlay);

    /// <summary>
    /// ��, ���� ��ų�� ��¥�� ����ϰ� �ϴ� �Լ�
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
    /// ��ų�� ����� �� ȣ��Ǵ� �Լ�
    /// </summary>
    public abstract void UseSkill();
}
