using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class OneWayDialogMarker : Marker, INotification
{
    // 씨네머신 (커스텀)마커를 통해 대화 csv 파일과 레터링 속도를 지정
    public TextAsset _oneWayDialog;
    public float _letteringSpeed;

    public PropertyName id => new PropertyName("One Way");
}
