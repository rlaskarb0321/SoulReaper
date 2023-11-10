using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    private AudioSource _audio;
    private AudioClip _currPlayBGM;

    // BGM 체인지 요구가 들어오면 원래 BGM의 볼륨을 서서히 낮추고 0 가까이 되면
    // 바꿔야하는 BGM으로 바꾼다. 그리고 서서히 볼륨을 원래치까지 올려준다.
    // 바꾸는 도중에 또 요구가 들어오면?
    // 딩이님한테 한번 여쭤봐야겠씀

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
