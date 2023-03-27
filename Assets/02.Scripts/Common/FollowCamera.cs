using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform _target;

    private Vector3 _camOriginPos;
    private Quaternion _camOriginRot;

    void Start()
    {
        _camOriginPos = transform.position;
        _camOriginRot = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_target == null)
            return;

        transform.position = new Vector3(_target.position.x, 0.0f, _target.position.z) + _camOriginPos;
        transform.rotation = _camOriginRot;
    }
}
