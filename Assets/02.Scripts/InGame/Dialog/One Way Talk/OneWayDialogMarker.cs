using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class OneWayDialogMarker : Marker, INotification
{
    public TextAsset _oneWayDialog;
    public float _letteringSpeed;

    public PropertyName id => new PropertyName("One Way");
}
