using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class PlayerBuff : ScriptableObject
{
    [SerializeField]
    private Sprite _buffImg;

    [Header("=== 버프 설명 ===")]
    public string _buffName;
    public string _description;
    public string _buffComment;
    public GameObject _effect;

    [Header("=== 버프 지속시간 ===")]
    [SerializeField]
    protected float _buffDur;

    [SerializeField]
    protected float _remainBuffDur;

    public Sprite BuffImg { get { return _buffImg; } set { _buffImg = value; } }
    public string BuffName { get { return _buffName; } }
    public float RemainBuffDur { get { return _remainBuffDur; } set { _remainBuffDur = value; } }
    public float BuffDur { get { return _buffDur; } }

    /// <summary>
    /// 플레이어의 다양한 스텟을 버프 시켜주는 함수
    /// </summary>
    public abstract void BuffPlayer();

    /// <summary>
    /// 플레이어에게 적영된 버프를 적용되기 전으로 돌리는 함수
    /// </summary>
    public abstract void ResetBuff();
}
