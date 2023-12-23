using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingDataEditor : MonoBehaviour
{
    [SerializeField]
    private SettingCategory _currOpenCategory;

    public void OpenCategory(SettingCategory category)
    {
        if (_currOpenCategory != null)
            _currOpenCategory.CloseContext();

        _currOpenCategory = category;
        category.OpenContext();
    }
}
