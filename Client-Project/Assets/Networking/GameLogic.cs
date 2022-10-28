using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic _singleton;
    public static GameLogic Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{ nameof(GameLogic)} instance already exists, duplicate destroyed");
                Destroy(value);
            }
        }
    }

    public GameObject playerPrefab;
    public Transform playerParent;
    public CinemachineVirtualCamera CMCam;

    private void Awake()
    {
        Singleton = this;
    }
}
