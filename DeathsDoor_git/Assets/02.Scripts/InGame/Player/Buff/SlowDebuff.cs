using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "DamageBuff", menuName = "CustomScriptableObject/DeBuff/SlowDebuff")]
public class SlowDebuff : PlayerBuff
{
    [Header("=== Slow Debuff ===")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("���� �̵��ӵ��� �ش� �ۼ�Ƽ����ŭ �̵��ӵ��� ����")]
    private float _slowPercent;

    private float _originMovSpeed;

    public override void BuffPlayer()
    {
        CharacterData.CData data = CharacterDataPackage._cDataInstance._characterData;
        float debuffMovSpeed = data._movSpeed - (data._movSpeed * _slowPercent);
        _originMovSpeed = data._movSpeed;

        UIScene._instance.UpdatePlayerMovSpeed(debuffMovSpeed);
    }

    public override void ResetBuff()
    {
        UIScene._instance.UpdatePlayerMovSpeed(_originMovSpeed);

        _remainBuffDur = _buffDur;
    }
}
