using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float respawnTime;
    public Vector3 deathPosition = new Vector3(0, -50, 0);

    private void Awake()
    {
        Singleton = this;
    }
}
