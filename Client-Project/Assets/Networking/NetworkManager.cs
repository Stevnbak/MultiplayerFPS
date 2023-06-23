using Cinemachine;
using Riptide;
using Riptide.Utils;
using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    public static NetworkManager Singleton
    {
        get => _singleton;
        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{ nameof(NetworkManager)} instance already exists, duplicate destroyed");
                Destroy(value);
            }
        }
    }
    [HideInInspector]
    public Client Client { get; private set; }
    [Header("Settings")]
    [SerializeField] bool test;

    //Networking values
    public string Ip = "127.0.0.1";
    public bool ConnectedToMatch = false;
    public ushort ConnectedServerId;
    [Header("Network Objects")]
    public Dictionary<ushort, PlayerNetworking> playerList = new Dictionary<ushort, PlayerNetworking>();

    public void Connect(string ip, string port, bool main = false)
    {
        if(main)
        {
            Singleton.Client.Connect($"{Ip}:2000");
            ConnectedToMatch = false;
        }
        else
        {
            Singleton.Client.Connect($"{ip}:{port}");
        }
    }

    private void DidConnect(object sender, EventArgs e)
    {
        if (ConnectedToMatch)
        {
            Debug.Log("Connected to match server");
            UIManager.Singleton.SendName();
        } else
        {
            Debug.Log("Connected to main server");
        }
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        if(ConnectedToMatch)
        {
            ConnectedToMatch = false;
            Singleton.Client.Connect($"{Ip}:2000");
            Debug.LogError("Couldn't connect to match server, returning to main!");
        }
        else
        {
            Debug.LogError("Couldn't connect to main server!");
        }
        UIManager.Singleton.BackToMain();
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        foreach(PlayerNetworking item in playerList.Values) {
            Destroy(item.gameObject);
        }
        playerList.Clear();
        if (!ConnectedToMatch)
        {
            Debug.Log("Disconnected from main server!");
            ConnectedToMatch = true;
        } else
        {
            Debug.Log("Disconnected from match server, returning to main!");
            ConnectedToMatch = false;
            Singleton.Client.Connect($"{Ip}:2000");
        }
        UIManager.Singleton.BackToMain();
    }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);
        Singleton.Client = new Client();
        Singleton.Client.Connected += DidConnect;
        Singleton.Client.ConnectionFailed += FailedToConnect;
        Singleton.Client.Disconnected += DidDisconnect;
        Debug.Log("Connecting to main server...");
        Singleton.Client.Connect($"{Ip}:2000");
    }

    private void FixedUpdate()
    {
        Client.Update();
    }

    private void OnApplicationQuit()
    {
       Client.Disconnect();
    }

    public static void SpawnPlayer(ushort id, string username, Vector3 position)
    {
        Debug.Log($"Spawned player with Id: {id} and Name: {username}");
        PlayerNetworking player;
        if (id == Singleton.Client.Id)
        {
            player = Instantiate(GameInformation.Singleton.playerPrefab, position, Quaternion.identity, GameInformation.Singleton.playerParent).GetComponent<PlayerNetworking>();
            player.IsLocal = true;
            GameInformation.Singleton.CMCam.Follow = player.transform.Find("PlayerCameraRoot");
        }
        else
        {
            player = Instantiate(GameInformation.Singleton.playerPrefab, position, Quaternion.identity, GameInformation.Singleton.playerParent).GetComponent<PlayerNetworking>();
            player.IsLocal = false;
            Destroy(player.GetComponent<PlayerController>());
            Destroy(player.GetComponent<PlayerInput>());
            Destroy(player.GetComponent<InputManager>());
        }

        player.name = $"Player {id} - {username}";
        player.Id = id;
        player.username = username;

        Singleton.playerList.Add(id, player);
    }
}
