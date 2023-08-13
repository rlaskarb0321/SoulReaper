using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : UIInteractBase
{
    public Color _selectedColor;

    private Color _originColor;
    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void Start()
    {
        _originColor = Color.white;
    }

    public override void OnPointerEnter()
    {
        Color color = _selectedColor;
        _text.color = color;
    }

    public override void OnPointerExit()
    {
        Color color = _originColor;
        _text.color = color;
    }
}
