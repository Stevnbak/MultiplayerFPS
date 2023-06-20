using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;


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
    public Server Server { get; private set; }
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;
    public bool active;
    [Header("Network Objects")]
    public Dictionary<ushort, PlayerNetworking> playerList = new Dictionary<ushort, PlayerNetworking>();

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
#else
        System.Console.Title = "Server";
        System.Console.Clear();
        Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
        RiptideLogger.Initialize(Debug.Log, true);
#endif
        Host();
    }

    public void Host()
    {
        Debug.Log("Starting server...");
        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
        Server.ClientConnected += PlayerConnected;
    }

    private void FixedUpdate()
    {
        Server.Update();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void PlayerConnected(object sender, ServerConnectedEventArgs e)
    {
        Debug.Log("Player connected, waiting for user information...");
    }

    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        Message message = Message.Create(MessageSendMode.Reliable, MessageIds.playerLeft);
        message.AddUShort(e.Client.Id);
        Singleton.Server.SendToAll(message);
        Destroy(playerList[e.Client.Id].gameObject);
    }

    public static void SpawnPlayer(ushort id, string username, Vector3 position)
    {
        Debug.Log($"Spawned player with Id: {id} and Name: {username}");
        PlayerNetworking player;
        player = Instantiate(GameInformation.Singleton.playerPrefab, position, Quaternion.identity, GameInformation.Singleton.playerParent).GetComponent<PlayerNetworking>();

        player.name = $"Player {id} - {username}";
        player.Id = id;
        player.username = username;

        Singleton.playerList.Add(id, player);
    }
}
