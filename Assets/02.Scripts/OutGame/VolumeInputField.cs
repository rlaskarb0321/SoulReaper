using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeInputField : MonoBehaviour
{
    [SerializeField]
    private GameObject _warning;

    // Field
    private TMP_InputField _inputField;

    private void Awake()
    {
        _inputField = GetComponent<TMP_InputField>();
    }

    public void EditVolume(Slider slider)
    {
        int value = int.Parse(_inputField.text);
        if (value < 0 || value > 100)
        {
            _warning.SetActive(false);
            _warning.SetActive(true);
            return;
        }

        slider.value = value * 0.01f;
    }
}
