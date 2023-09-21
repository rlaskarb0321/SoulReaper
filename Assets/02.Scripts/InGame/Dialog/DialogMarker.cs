using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogMarker : Marker, INotification
{
    public TextAsset _dialogCSV;
    public float _letteringSpeed;

    public PropertyName id => new PropertyName();
}
