using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class PlayerBuff : ScriptableObject
{
    [SerializeField]
    private Sprite _buffImg;

    [SerializeField]
    protected string _buffName;

    [SerializeField]
    protected float _buffDur;

    [SerializeField]
    protected float _remainBuffDur;

    protected WaitForSeconds _ws;

    public Sprite BuffImg { get { return _buffImg; } set { _buffImg = value; } }
    public string BuffName { get { return _buffName; } }
    public float RemainBuffDur { get { return _remainBuffDur; } set { _remainBuffDur = value; } }
    public float BuffDur { get { return _buffDur; } }

    public PlayerBuff()
    {
        _ws = new WaitForSeconds(1.0f);
    }

    /// <summary>
    /// �÷��̾��� �پ��� ������ ���� �����ִ� �Լ�
    /// </summary>
    public abstract void BuffPlayer();

    /// <summary>
    /// �÷��̾�� ������ ������ ����Ǳ� ������ ������ �Լ�
    /// </summary>
    public abstract void ResetBuff();

    /// <summary>
    /// ���� ����� ������ ���ӽð��� ��� �Լ�
    /// </summary>
    /// <returns></returns>
    public IEnumerator DecreaseBuffDur(TMP_Text text)
    {
        text.text = _remainBuffDur.ToString();

        while (_remainBuffDur > 0.0f)
        {
            yield return _ws;
            _remainBuffDur -= 1.0f;
            text.text = _remainBuffDur.ToString();
        }

        text.text = _remainBuffDur.ToString();
        //if (_remainBuffDur <= 0.0f)
        //{
        //    ResetBuff();
        //}
    }
}
