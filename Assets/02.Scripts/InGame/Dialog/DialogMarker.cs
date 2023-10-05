using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogMarker : Marker, INotification
{
    // 여기에 선택지 옵션에 따른 결과를 int 로 저장시키고
    // _dialogCSV 를 TextAsset[] 으로 만든 후, int 값에 맞는 대화문을 가져가도록 한다.
    // 선택지에 따른 반응을 다르게 해야할 필요가 없는 대화문을 구별하기위해, isOption 이라는 변수를 통해
    // 선택지의 경우 해당 bool값을 true 로 바꾸고, true 일때만 배열의 값을 가져오도록 한다.
    // 그 이외의 false 인 경우에는 무조건 0 번째 값을 가져옴

    // ui 만들기
    // 일반 대화인지 선택지 대화인지 bool 변수
    // scroll view 동적 추가
    // 일반 대화와 선택지 대화 껐다 켜서 왔다갔다하기

    public TextAsset _dialogCSV;
    public float _letteringSpeed;
    public enum eDialogType { Normal, Selection, Victim }
    public eDialogType _dialogType;

    public PropertyName id => new PropertyName();
}
