using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum eState { Idle, Falling, Moving, Attack, Hit, }

    [SerializeField] private eState _state;
    public eState State { get { return _state; } set { _state = value; } }

    private void Awake()
    {
        _state = eState.Idle;
    }

    public void SetStateIdle()
    {
        _state = eState.Idle;
    }
}
