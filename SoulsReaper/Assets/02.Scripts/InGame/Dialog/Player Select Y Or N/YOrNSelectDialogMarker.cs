using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class YOrNSelectDialogMarker : Marker, INotification
{
    public TextAsset[] _playerSelection;

    public PropertyName id => new PropertyName("Selection");
}
