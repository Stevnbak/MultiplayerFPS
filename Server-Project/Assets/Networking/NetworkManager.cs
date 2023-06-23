using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkManager : MonoBehaviour
{
    public Server Server { get; private set; }
    [SerializeField]
    public ushort Id { get; private set; }
    public ushort Port { get; private set; }
    [Header("Network Objects")]
    public Dictionary<ushort, PlayerNetworking> playerList = new Dictionary<ushort, PlayerNetworking>();
    public Transform playerParent;

    public void Host(ushort port, ushort serverId)
    {
        Debug.Log("Starting server...");
        Server = new Server();
        Server.Start(port, GameInformation.Singleton.maxUsersPerLobby);
        Server.ClientDisconnected += PlayerLeft;
        Server.ClientConnected += PlayerConnected;
        Port = port;
        Id = serverId;
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
        Server.SendToAll(message);
        Destroy(playerList[e.Client.Id].gameObject);
        playerList.Remove(e.Client.Id);
        if(playerList.Count == 0)
        {
            Debug.Log($"Match server {Id} closing...");
            ServerManager.Singleton.CloseServer(Id);
        }
    }

    public static void SpawnPlayer(ushort playerId, ushort serverId, string username, Vector3 position)
    {
        NetworkManager networkManager = ServerManager.Singleton.ActiveServers[serverId];
        Debug.Log($"Spawned player with Id: {playerId} and Name: {username}");
        PlayerNetworking player;
        player = Instantiate(GameInformation.Singleton.playerPrefab, position, Quaternion.identity, networkManager.playerParent).GetComponent<PlayerNetworking>();

        player.name = $"Player {playerId} - {username}";
        player.Id = playerId;
        player.serverId = serverId;
        player.username = username;

        networkManager.playerList.Add(playerId, player);
    }
}
