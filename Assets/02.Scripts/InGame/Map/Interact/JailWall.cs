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
    private GameObject _torqueObj;

    [SerializeField]
    private GameObject _jailWall;

    [SerializeField]
    private AudioClip[] _lockAudio;

    [SerializeField]
    private AudioClip _jailAudio;

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
            _audio.PlayOneShot(_lockAudio[(int)eAudio.Cant_Unlock]);
            _isInteract = false;
            return;
        }

        Unlock();
        _data.EditData();
    }

    private void Unlock()
    {
        Rigidbody rbody = _torqueObj.GetComponent<Rigidbody>();
        BoxCollider coll = _torqueObj.GetComponent<BoxCollider>();
        Vector3 randomTorque = (Random.insideUnitSphere - _torqueObj.transform.position).normalized;

        coll.isTrigger = true;
        rbody.isKinematic = false;
        rbody.AddTorque(randomTorque * 9.0f, ForceMode.Impulse);

        _audio.PlayOneShot(_lockAudio[(int)eAudio.Can_Unlock]);
        this.GetComponent<BoxCollider>().enabled = false;
        SetActiveInteractUI(false);
        StartCoroutine(DeactiveObj());
    }

    private IEnumerator DeactiveObj()
    {
        yield return new WaitForSeconds(1.0f);

        Animator jailWall = _jailWall.GetComponent<Animator>();
        jailWall.enabled = true;
        _audio.PlayOneShot(_jailAudio);

        yield return new WaitForSeconds(4.0f);

        gameObject.SetActive(false);
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatGameObjectUI(UIScene._instance._interactUI, value, pos, _interactName);
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
