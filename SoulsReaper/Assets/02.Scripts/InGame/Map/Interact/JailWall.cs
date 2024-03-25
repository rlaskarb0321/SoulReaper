using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JailWall : MonoBehaviour, IInteractable
{
    [Header("=== 상호작용 ===")]
    [SerializeField]
    private string _interactName;

    [SerializeField]
    private Transform _floatUIPos;

    [Header("=== 자물쇠 오브젝트 ===")]
    [SerializeField]
    private GameObject _torqueObj;

    [SerializeField]
    private GameObject _jailWall;

    [SerializeField]
    private AudioClip[] _lockAudio;

    [SerializeField]
    private AudioClip _jailAudio;

    [SerializeField]
    private string _alarmText;

    [SerializeField]
    private float _torquePower;

    [Header("=== 맵 데이터 ===")]
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
            UIScene._instance.FloatAlarmTextUI(_alarmText);
            _audio.PlayOneShot(_lockAudio[(int)eAudio.Cant_Unlock], _audio.volume * SettingData._sfxVolume);
            _isInteract = false;
            return;
        }

        Unlock();
    }

    public void SetActiveInteractUI(bool value)
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(_floatUIPos.position);
        UIScene._instance.FloatInteractTextUI(UIScene._instance._interactUI, value, pos, _interactName);
    }

    /// <summary>
    /// 자물쇠를 따주는 메서드
    /// </summary>
    private void Unlock()
    {
        Rigidbody rbody = _torqueObj.GetComponent<Rigidbody>();
        BoxCollider coll = _torqueObj.GetComponent<BoxCollider>();
        Vector3 randomTorque = (Random.insideUnitSphere - _torqueObj.transform.position).normalized;

        coll.isTrigger = true;
        rbody.isKinematic = false;
        rbody.AddTorque(randomTorque * _torquePower, ForceMode.Impulse);

        this.GetComponent<BoxCollider>().enabled = false;
        _audio.PlayOneShot(_lockAudio[(int)eAudio.Can_Unlock], _audio.volume * SettingData._sfxVolume);
        _data.EditData();
        SetActiveInteractUI(false);
        StartCoroutine(DeactiveSelf());
    }

    /// <summary>
    /// 감옥 문 해제 연출 후 일정 시간 뒤 전체 오브젝트 비활성화
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeactiveSelf()
    {
        yield return new WaitForSeconds(1.0f);

        Animator jailWall = _jailWall.GetComponent<Animator>();
        jailWall.enabled = true;
        _audio.PlayOneShot(_jailAudio, _audio.volume * SettingData._sfxVolume);

        yield return new WaitForSeconds(4.0f);

        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        if (Input.GetKey(KeyCode.F))
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
