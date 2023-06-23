using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject playerPrefab;
    public float respawnTime;
    public Vector3 deathPosition = new Vector3(0, -50, 0);
    public List<WeaponScript> WeaponPrefabList = new List<WeaponScript>();
    public ushort maxUsersPerLobby = 10;

    private void Awake()
    {
        Singleton = this;
    }
}
