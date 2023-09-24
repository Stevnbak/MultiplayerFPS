using Riptide;
using Riptide.Transports;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    private static ServerManager _singleton;
    public static ServerManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ServerManager)} instance already exists, duplicate destroyed");
                Destroy(value);
            }
        }
    }
    private void Awake()
    {
        Singleton = this;
    }
    public Server MainServer { get; private set; }
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
        //Start server
        MainServer = new Server();
        MainServer.Start(2000, 1000); //Port 2000, max 1000 connections
    }
    private void FixedUpdate()
    {
        MainServer.Update();
    }
    private void OnApplicationQuit()
    {
        MainServer.Stop();
    }

    //Server list
    public Dictionary<ushort, NetworkManager> ActiveServers = new();

    public void StartNewServer(ushort fromClientId)
    {
        Debug.Log("Loading new scene");
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneAt(SceneManager.loadedSceneCount);
        StartCoroutine(waitForSceneLoad(scene, fromClientId));
    }
    private IEnumerator waitForSceneLoad(Scene scene, ushort fromClientId)
    {
        //Wait for scene to load
        while(!scene.isLoaded)
        {
            yield return null;
        }

        //Scene has been loaded
        ushort newPort = (ushort)Mathf.RoundToInt(Random.Range(2001, 10000));
        ushort serverId = (ushort)ActiveServers.Count;

        Debug.Log($"Scene {scene.name} ({SceneManager.loadedSceneCount}) loaded!");

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("GameController"))
        {
            Debug.Log(g.name + " - " + g.scene.name);
        }
        NetworkManager networkManager = GameObject.FindGameObjectsWithTag("GameController").Last().GetComponent<NetworkManager>();
        Debug.Log(networkManager);

        /**
        GameObject gameObject = Instantiate(new GameObject(), transform);
        gameObject.name = "Match server " + serverId;
        NetworkManager networkManager = gameObject.AddComponent<NetworkManager>();*/

        networkManager.Host(newPort, serverId);
        ActiveServers.Add(serverId, networkManager);
        Debug.Log("Started server with id " + serverId);
        Message newMessage = Message.Create(MessageSendMode.Reliable, MessageIds.connectToServer);
        newMessage.AddUShort(networkManager.Port);
        newMessage.AddUShort(networkManager.Id);
        MainServer.Send(newMessage, fromClientId);
        Debug.Log("User connected to new match server " + networkManager.Id);
    }


    public void CloseServer(ushort serverId)
    {
        NetworkManager server = GetServer(serverId);
        if(server == null) return;
        server.Server.Stop();
        ActiveServers.Remove(serverId);
        SceneManager.UnloadSceneAsync(server.gameObject.scene);
        Debug.Log("Closed server with id " + serverId);
    }
    public NetworkManager GetServer(ushort serverId)
    {
        if(ActiveServers.ContainsKey(serverId)) return ActiveServers[serverId];
        else
        {
            Debug.LogWarning($"Tried to get server with id {serverId} but couldn't find it!");
            return null;
        }
    }
    public NetworkManager GetServerFromPlayer(ushort playerId)
    {
        NetworkManager server = ActiveServers.Values.ToList().Find((s) => s.playerList.ContainsKey(playerId));
        if (server == null)
        {
            Debug.LogWarning($"Tried to get server from player with id {playerId} but couldn't find any!");
            return null;
        }
        return server;
    }
}
