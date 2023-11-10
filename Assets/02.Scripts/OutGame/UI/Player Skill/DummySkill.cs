using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySkill : MonoBehaviour
{
    [HideInInspector]
    public Transform _originParent;

    private bool _isFollow;
    private RectTransform _originParentRect;
    private RectTransform _rect;

    private void Awake()
    {
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
        transform.SetParent(parent);
    }

    public void SwapSkillPos(Transform parent)
    {
        _originParent.SetParent(parent);
        _originParent.SetAsFirstSibling();
        _originParentRect.anchoredPosition = Vector2.zero;
        _rect.anchoredPosition = Vector2.zero;
    }

    public void ToOriginPos()
    {
        _isFollow = false;
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