using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour
{ 
    [Header("Settings")]
    public bool position;
    public bool rotation;

    //Information
    [Header("Client Information")]
    public ushort Id;
    public bool IsLocal;
    public string username;

    [Header("Player Information")]
    public string state;
    public PlayerInfo playerInfo;

    private void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
    }

    void FixedUpdate()  
    {
        //Only on local player...
        if (!IsLocal) return;

        //Player is dead, no updates to send
        if (!playerInfo.alive) return;

        //Send updated position
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageIds.playerTransformUpdate);
        if (position) message.AddVector3(transform.position); else message.AddVector3(Vector3.zero);
        if (rotation) message.AddQuaternion(transform.rotation); else message.AddQuaternion(Quaternion.identity);
        NetworkManager.Singleton.Client.Send(message);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.playerList.Remove(Id);
    }
}
