using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySkill : MonoBehaviour
{
    [Header("=== Grab / Abandon Sound Clip ===")]
    [SerializeField]
    private AudioClip[] _grabSound;

    [HideInInspector]
    public Transform _originParent;

    private enum eGrabSound { Grab, Abandon, Swap, Count, }
    private bool _isFollow;
    private RectTransform _originParentRect;
    private RectTransform _rect;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _rect = GetComponent<RectTransform>();
        _originParent = transform.parent;
        _originParentRect = _originParent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Follow();
    }

    public void FollowMouse(Transform parent)
    {
        if (parent == null)
        {
            print("���� �̹����� �θ������Ʈ�� null �̸� �ȵȴ�.");
            return;
        }

        _isFollow = true;
        _audio.PlayOneShot(_grabSound[(int)eGrabSound.Grab]);
        transform.SetParent(parent);
    }

    public void SwapSkillPos(Transform parent)
    {
        _originParent.SetParent(parent);
        _originParent.SetAsFirstSibling();
        _originParentRect.anchoredPosition = Vector2.zero;
        _rect.anchoredPosition = Vector2.zero;
        _audio.PlayOneShot(_grabSound[(int)eGrabSound.Swap]);
    }

    public void ToOriginPos()
    {
        _isFollow = false;
        _audio.PlayOneShot(_grabSound[(int)eGrabSound.Abandon]);
        transform.SetParent(_originParent);
        transform.SetAsFirstSibling();
    }

    private void Follow()
    {
        if (_isFollow)
        {
            _rect.position = Input.mousePosition;
            return;
        }

        // ���� �ִ� �ڸ��� ������ ���������ʰ�, �׷��� ������ ���콺���� ���� �ڸ��� �Ű�����
        if (_rect.anchoredPosition == Vector2.zero)
            return;

        if (Vector2.Distance(_rect.anchoredPosition, Vector2.zero) <= 1.0f)
        {
            _rect.anchoredPosition = Vector2.zero;
            return;
        }

        // ������ġ�� �ű��
        _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, Vector2.zero, 0.1f);
    }
}