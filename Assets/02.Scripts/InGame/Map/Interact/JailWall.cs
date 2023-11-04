using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailWall : MonoBehaviour, IInteractable
{
    [Header("=== Interact ===")]
    [SerializeField]
    private string _interactName;

    [SerializeField]
    private Transform _floatUIPos;

    [Header("=== Lock Obj ===")]
    [SerializeField]
    private GameObject[] _torqueObj;

    [SerializeField]
    private AudioClip[] _audioResource;

    [Header("=== Data ===")]
    [SerializeField]
    private DataApply _data;

    // Field
    private AudioSource _audio;
    private bool _isInteract;
    private enum eAudio { Can_Unlock, Cant_Unlock }

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (_isInteract)
            return;

        _isInteract = true;
        if (!MapDataPackage._mapData._castleA._data._isYellowKeyGet)
        {
            _audio.PlayOneShot(_audioResource[(int)eAudio.Cant_Unlock]);
            _isInteract = false;
            return;
        }

        Unlock();
        _data.EditData();
    }

    private void Unlock()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        SetActiveInteractUI(false);
        for (int i = 0; i < _torqueObj.Length; i++)
        {
            Rigidbody rbody = _torqueObj[i].GetComponent<Rigidbody>();
            BoxCollider coll = _torqueObj[i].GetComponent<BoxCollider>();
            Vector3 randomTorque = (Random.insideUnitSphere - _torqueObj[i].transform.position).normalized;

            coll.isTrigger = true;
            rbody.isKinematic = false;
            rbody.AddTorque(randomTorque * 5.0f, ForceMode.Impulse);
        }

        _audio.PlayOneShot(_audioResource[(int)eAudio.Can_Unlock]);
        StartCoroutine(DeactiveObj());
    }

    private IEnumerator DeactiveObj()
    {
        yield return new WaitForSeconds(3.0f);

        gameObject.SetActive(false);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractUI(value, pos, _interactName);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
            return;
        }

        SetActiveInteractUI(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        SetActiveInteractUI(false);
    }
}
