using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogMarker : Marker, INotification
{
    // 여기에 선택지 옵션에 따른 결과를 int 로 저장시키고
    // _dialogCSV 를 TextAsset[] 으로 만든 후, int 값에 맞는 대화문을 가져가도록 한다.

    public TextAsset _dialogCSV;
    public float _letteringSpeed;

    public PropertyName id => new PropertyName();
}
