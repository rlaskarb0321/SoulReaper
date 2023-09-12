using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldMonster
{
    public void Scout();
    public Vector3 SetRandomPoint();
}

public interface IKeyMonster
{

}

public class MonsterAI_1 : MonoBehaviour
{
    public MonsterBase _monster;
    public Transform _target;

    protected virtual void Combat() { }
}
