using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySkill : MonoBehaviour
{
    private bool _isFollow;
    private Transform _originParent;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _originParent = transform.parent;
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

        _rect.anchoredPosition = Vector2.Lerp(_rect.anchoredPosition, Vector2.zero, 0.1f);
    }
}