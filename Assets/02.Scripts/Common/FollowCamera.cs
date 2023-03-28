using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //public Transform _target;

    //private Vector3 _targetOriginPos;
    //private Vector3 _camOriginPos;
    //private Quaternion _camOriginRot;

    public Transform _target;
    private Vector3 _camOriginPos;

    void Start()
    {
        //_camOriginPos = transform.position;
        //_camOriginRot = transform.rotation;
        //_targetOriginPos = _target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if (_target == null)
        //    return;

        //transform.position 
        //    = new Vector3(_target.position.x + _camOriginPos.x, _camOriginPos.y,
        //    _target.position.z + _camOriginPos.z);
        //transform.rotation = _camOriginRot;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += new Vector3(0.5f, 0.0f, 0.0f);
        }
        else
        {
            transform.position = _target.position;
        }
    }
}
