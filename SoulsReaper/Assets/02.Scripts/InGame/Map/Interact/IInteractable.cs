using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// ��ȣ�ۿ� �� ȣ���� �޼���
    /// </summary>
    public void Interact();

    /// <summary>
    /// ��ȣ�ۿ� Ű UI�� ���� ���ִ� �޼���
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveInteractUI(bool value);
}