using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 플레이어, 적, 희생자의 일방적인 대화와 플레이어의 y/n 대답에 관한 대화를 담당
// 만일 선택지가 더 다양해지거나 플레이어의 입력방식이 다양해질 경우, 마커를 상속받는 다른 스크립트를 하나 더 만들면 된다.
public class DialogMarker : Marker, INotification
{
    public TextAsset[] _dialogCSV_1;
    public float _letteringSpeed;
    public enum eDialogType { Normal, Selection, Victim }
    public eDialogType _dialogType;

    public PropertyName id => new PropertyName();
}
