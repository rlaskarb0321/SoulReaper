using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuffICon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("=== 지속시간 & 이미지 ===")]
    public TMP_Text _durationText;
    public Image _buffImamge;

    [Header("=== 버프 설명 UI ===")]
    public GameObject _descriptionObj;
    public TMP_Text _titleText;
    public TMP_Text _descriptionText;
    public TMP_Text _commentText;

    [Header("=== 버프 획득 시 ===")]
    [SerializeField]
    private float _floatTime;

    // Field
    private RectTransform _rect;
    private PlayerBuff _buff;
    private float _floatTimeOrigin;
    private bool _isFirstAlarm;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

        _floatTimeOrigin = _floatTime;
    }

    private void Update()
    {
        if (!_isFirstAlarm)
            return;
        if (_floatTime <= 0.0f)
        {
            _floatTime = 0.0f;
            _isFirstAlarm = false;
            _descriptionObj.SetActive(false);
            return;
        }

        _floatTime -= Time.deltaTime;
    }

    public void InitBuff(PlayerBuff buff, bool isSceneReload)
    {
        _buff = buff;
        _floatTime = _floatTimeOrigin;
        _isFirstAlarm = !isSceneReload;

        _titleText.text = buff.BuffName;
        _descriptionText.text = buff._description;
        _commentText.text = buff._buffComment;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
        _descriptionObj.SetActive(!isSceneReload);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isFirstAlarm)
        {
            _isFirstAlarm = false;
            _descriptionObj.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
            _descriptionObj.SetActive(true);
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
        _descriptionObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionObj.SetActive(false);
    }
}
