using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    private AudioSource _audio;
    private AudioClip _currPlayBGM;

    // BGM ü���� �䱸�� ������ ���� BGM�� ������ ������ ���߰� 0 ������ �Ǹ�
    // �ٲ���ϴ� BGM���� �ٲ۴�. �׸��� ������ ������ ����ġ���� �÷��ش�.
    // �ٲٴ� ���߿� �� �䱸�� ������?
    // ���̴����� �ѹ� ������߰ھ�

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    public void ChangeBGM(AudioClip changeBGM)
    {

    }
}
