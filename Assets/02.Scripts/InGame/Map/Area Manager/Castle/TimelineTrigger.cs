using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _data;

    private PlayableDirector _playableDirector;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayerTeam"))
            return;

        ProductionMgr.StartProduction(_playableDirector);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        _data.EditData();
    }
}
