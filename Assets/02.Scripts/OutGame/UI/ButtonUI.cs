using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : UIInteractBase
{
    public Color _selectedColor;
    public Color _originColor;
    public AudioClip _onPointerSound;

    private AudioSource _audio;
    private TMP_Text _text;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _text = GetComponent<TMP_Text>();
    }

    public override void OnPointerEnter()
    {
        Color color = _selectedColor;
        _text.color = color;
        _audio.PlayOneShot(_onPointerSound);
    }

    public override void OnPointerExit()
    {
        Color color = _originColor;
        _text.color = color;
    }
}
