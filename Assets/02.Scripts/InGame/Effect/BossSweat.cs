using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSweat : MonoBehaviour
{
    [SerializeField]
    private GameObject _refGameObject;

    private void Update()
    {
        this.transform.position = _refGameObject.transform.position;
        this.transform.rotation = _refGameObject.transform.rotation;
    }
}
