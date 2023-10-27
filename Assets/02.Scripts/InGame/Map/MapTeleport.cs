using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapTeleport : MonoBehaviour
{
    [Header("=== From ===")]
    [SerializeField] private GameObject _enterPos;
    [SerializeField] private GameObject _exitPos;

    [Header("=== To ===")]
    [SerializeField] private GameObject _nextPos;

    // Field
    private PlayerMove_1 _playerMove;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerTeam"))
        {
            if (_playerMove == null)
                _playerMove = other.GetComponent<PlayerMove_1>();
            if (_playerMove._state.State == PlayerFSM.eState.Ladder)
            {
                _playerMove.ClimbDown();
            }

            _playerMove.TeleportPlayer(_nextPos.transform);
            EditMapData();
        }
    }

    private void EditMapData()
    {
        print("character teleport data save");

        CharacterData.CData data = CharacterDataPackage._characterData._characterData;
        data._pos = _nextPos.transform.position;
        CharacterDataPackage._characterData._characterData = data;
        DataManage.SaveCData(CharacterDataPackage._characterData, "TestCData");
    }
}
