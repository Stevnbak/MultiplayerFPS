using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameInformation : MonoBehaviour
{
    private static GameInformation _singleton;
    public static GameInformation Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{ nameof(GameInformation)} instance already exists, duplicate destroyed");
                Destroy(value);
            }
        }
    }

    [Header("Scene configuration")]
    public GameObject playerPrefab;
    public Transform tempParent;
    public Transform playerParent;
    public CinemachineVirtualCamera CMCam;

    private void Awake()
    {
        Singleton = this;
    }
}
