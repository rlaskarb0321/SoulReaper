using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 플레이어가 대화 선택지를 고를때 사용하는 커스텀 마커
/// </summary>
public class YOrNSelectDialogMarker : Marker, INotification
{
    public TextAsset[] _playerSelection;

    public PropertyName id => new PropertyName("Selection");
}
