using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIInteractBase : MonoBehaviour
{
    public virtual void OnClickBtn() { }

    public virtual void OnPointerEnter() { }

    public virtual void OnPointerExit() { }
}