using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossInduce : MonoBehaviour, IYOrNSelectOption
{
    private int _selectNum;
    private PlayableDirector _playableDirector;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    public void ApplyOption(int selectNum)
    {
        _selectNum = selectNum;
    }

    public void CheckAnswer(bool isAnswerYes)
    {
        // Y �Ǵ� N ������� ������ �ٸ����ϰ� Playable�� �����Ŵ

        _playableDirector.Resume();
    }

    public int ReturnSelectResult()
    {
        return _selectNum;
    }
}
