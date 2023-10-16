using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScene : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetUIActive(_pausePanel);
        }
    }

    public void SetUIActive(GameObject uiObj)
    {
        bool isOn = uiObj.activeSelf;
        uiObj.SetActive(!isOn);
    }
}
