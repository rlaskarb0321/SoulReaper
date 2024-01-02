using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AfterYOrNSelectDialogMarker : Marker, INotification
{
    public TextAsset[] _dialogCSVFile;
    public float _letteringSpeed;

    public PropertyName id => new PropertyName("After Y Or N");
}