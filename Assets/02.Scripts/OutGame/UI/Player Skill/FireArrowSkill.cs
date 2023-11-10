using UnityEngine;
using UnityEngine.EventSystems;

public class FireArrowSkill : PlayerSkill, IOnOffSwitchSkill
{
    public enum eSkillActiveState { DeActive, Active, Count, }
    public eSkillActiveState _skillActiveState;

    [Header("=== On/Off Sound Clip ===")]
    [SerializeField]
    private AudioClip[] _onOffSwitchSound;

    [Header("=== Select Border ===")]
    [SerializeField]
    private GameObject _selectBorder;

    // Field
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _currCoolDown = _coolDown;
        _coolDownText.text = ((int)_currCoolDown).ToString();
    }

    private void Update()
    {
        CoolDownSkill();
    }

    public void SwitchSkillActive(bool isAudioPlay)
    {
        // 스킬의 On Off 상태를 바꿈
        _skillActiveState++;
        _skillActiveState = (eSkillActiveState)((int)_skillActiveState % (int)eSkillActiveState.Count);
        _selectBorder.SetActive(_skillActiveState == eSkillActiveState.Active);

        if (isAudioPlay)
        {
            _audio.PlayOneShot(_onOffSwitchSound[(int)_skillActiveState]);
        }
        _player.ActiveFireArrow(_skillActiveState, this);
    }

    public override void UseSkill()
    {
        if (_isCoolDown)
        {
            print("is cool down");
            return;
        }

        SwitchSkillActive(true);
    }

    public void UseOnOffSkill()
    {
        SwitchSkillActive(false);

        _isCoolDown = true;
        _coolDownPanel.gameObject.SetActive(true);
    }

    private void CoolDownSkill()
    {
        if (!_isCoolDown)
            return;

        if (_currCoolDown <= 0.0f)
        {
            _coolDownPanel.gameObject.SetActive(false);
            _coolDownPanel.fillAmount = 1.0f;

            _coolDownText.text = ((int)_currCoolDown).ToString();
            _coolDownText.gameObject.SetActive(false);

            _currCoolDown = _coolDown;
            _isCoolDown = false;
            return;
        }

        _coolDownText.gameObject.SetActive(true);
        _currCoolDown -= Time.deltaTime;
        _coolDownText.text = _currCoolDown <= 1.0f ? _currCoolDown.ToString("0.0") : ((int)_currCoolDown).ToString();
        _coolDownPanel.fillAmount = (_currCoolDown / _coolDown);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // print("Show Skill Inform");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // print("Hide Skill Inform");
    }
}
