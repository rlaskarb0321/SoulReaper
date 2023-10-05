using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogMarker : Marker, INotification
{
    // ���⿡ ������ �ɼǿ� ���� ����� int �� �����Ű��
    // _dialogCSV �� TextAsset[] ���� ���� ��, int ���� �´� ��ȭ���� ���������� �Ѵ�.
    // �������� ���� ������ �ٸ��� �ؾ��� �ʿ䰡 ���� ��ȭ���� �����ϱ�����, isOption �̶�� ������ ����
    // �������� ��� �ش� bool���� true �� �ٲٰ�, true �϶��� �迭�� ���� ���������� �Ѵ�.
    // �� �̿��� false �� ��쿡�� ������ 0 ��° ���� ������

    // ui �����
    // �Ϲ� ��ȭ���� ������ ��ȭ���� bool ����
    // scroll view ���� �߰�
    // �Ϲ� ��ȭ�� ������ ��ȭ ���� �Ѽ� �Դٰ����ϱ�

    public TextAsset _dialogCSV;
    public float _letteringSpeed;
    public enum eDialogType { Normal, Selection, Victim }
    public eDialogType _dialogType;

    public PropertyName id => new PropertyName();
}
