using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogMarker : Marker, INotification
{
    public TextAsset[] _dialogCSV_1;
    public float _letteringSpeed;
    public enum eDialogType { Normal, Selection, Victim }
    public eDialogType _dialogType;

    public PropertyName id => new PropertyName();
}
