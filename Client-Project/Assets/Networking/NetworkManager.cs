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
    public Client Client { get; private set; }
    [SerializeField] private string ip;
    [SerializeField] private ushort port;
    [Header("Network Objects")]
    public static Dictionary<ushort, PlayerNetworking> playerList = new Dictionary<ushort, PlayerNetworking>();

    public void Connect()
    {
        Singleton.Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        Debug.Log("Connected to server");
        UIManager.Singleton.SendName();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
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
            player = Instantiate(GameLogic.Singleton.playerPrefab, position, Quaternion.identity, GameLogic.Singleton.playerParent).GetComponent<PlayerNetworking>();
            player.IsLocal = true;
            GameLogic.Singleton.CMCam.Follow = player.transform.Find("PlayerCameraRoot");
        }
        else
        {
            player = Instantiate(GameLogic.Singleton.playerPrefab, position, Quaternion.identity, GameLogic.Singleton.playerParent).GetComponent<PlayerNetworking>();
            player.IsLocal = false;
            Destroy(player.GetComponent<FirstPersonController>());
            Destroy(player.GetComponent<PlayerInput>());
            Destroy(player.GetComponent<StarterAssetsInputs>());
        }

        player.name = $"Player {id} - {username}";
        player.Id = id;
        player.username = username;

        playerList.Add(id, player);
    }
}
