using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// 상호작용 시 호출할 메서드
    /// </summary>
    public void Interact();

    /// <summary>
    /// 상호작용 키 UI를 띄우게 해주는 메서드
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveInteractUI(bool value);
}