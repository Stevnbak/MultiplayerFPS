using Riptide;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MessageHandling : MonoBehaviour
{

    //A player joined the game
    [MessageHandler((ushort)MessageIds.playerJoined)]
    static void playerJoined(Message message)
    {
        NetworkManager.SpawnPlayer(message.GetUShort(), message.GetString(), message.GetVector3());
    }

    //Another player left the game
    [MessageHandler((ushort)MessageIds.playerLeft)]
    static void playerLeft(Message message)
    {
        ushort id = message.GetUShort();
        Debug.Log($"Player with id {id} left the game");
        Destroy(NetworkManager.Singleton.playerList[id].gameObject);
    }

    //Get updated position (Not local)
    [MessageHandler((ushort)MessageIds.playerTransformUpdate)]
    static void playerTransformUpdate(Message message)
    {
        ushort id = message.GetUShort();
        if(!NetworkManager.Singleton.playerList.ContainsKey(id))
        {
            Debug.LogWarning($"Couldn't find player with id {id} in the player list...");
            return;
        }
        if (NetworkManager.Singleton.playerList[id].IsLocal) return;
       // Debug.Log($"Player transform updated for {id}");
        Transform transform = NetworkManager.Singleton.playerList[id].transform;
        Vector3 position = message.GetVector3();
        if (position != Vector3.zero)
            transform.position = position;
        Quaternion rotation = message.GetQuaternion();
        if (rotation != Quaternion.identity)
            transform.rotation = rotation;
    }

    //Recieve status update
    [MessageHandler((ushort)MessageIds.playerStatusUpdate)]
    static void playerStatusUpdate(Message message)
    {
        ushort id = message.GetUShort();
        string newState = message.GetString();
        Debug.Log($"Changed state for {id} to {newState}");
        NetworkManager.Singleton.playerList[id].state = newState;
    }
}
