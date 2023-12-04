using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���̺� �������� ���͸� ��ȯ�� �� ���� ��ũ��Ʈ
/// </summary>
public class WaveMonsterSummon : MonoBehaviour
{
    public GameObject _monster;

    private IDisolveEffect _dissolve;

    private void Awake()
    {
        _dissolve = _monster.GetComponent<IDisolveEffect>();
    }

    public void SetMonsterAnimOn()
    {
        StartCoroutine(_dissolve.DissolveAppear());
    }

    public void SetMonsterAIOn()
    {
        _dissolve.CompleteSummon();
        gameObject.SetActive(false);
    }
}
