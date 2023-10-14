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
        // Y 또는 N 상관없이 반응만 다르게하고 Playable은 진행시킴

        _playableDirector.Resume();
    }

    public int ReturnSelectResult()
    {
        return _selectNum;
    }
}
