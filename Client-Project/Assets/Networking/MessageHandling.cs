using Riptide;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MessageHandling : MonoBehaviour
{

    //Connected to match
    [MessageHandler((ushort)MessageIds.connectToServer)]
    static void connectedToMatch(Message message)
    {
        Debug.Log("Got a match!");
        ushort port = message.GetUShort();
        ushort serverId = message.GetUShort();
        NetworkManager.Singleton.Client.Disconnect();
        NetworkManager.Singleton.ConnectedServerId = serverId;
        NetworkManager.Singleton.Connect(NetworkManager.Singleton.Ip, port.ToString());
    }
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
        PlayerNetworking player = NetworkManager.Singleton.playerList[id];
        if (player.IsLocal && player.playerInfo.alive) return;
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

    //Recieve health update
    [MessageHandler((ushort)MessageIds.playerHealthUpdate)]
    static void playerHealthUpdate(Message message)
    {
        ushort id = message.GetUShort();
        float newHealth = message.GetFloat();
        NetworkManager.Singleton.playerList[id].gameObject.GetComponent<PlayerInfo>().UpdateHealth(newHealth);
    }

    //Player Death
    [MessageHandler((ushort)MessageIds.playerDeath)]
    static void playerDeath(Message message)
    {
        ushort id = message.GetUShort();
        Debug.Log("Player " + id + " died");
        NetworkManager.Singleton.playerList[id].gameObject.GetComponent<PlayerInfo>().Death();
    }

    //Player Respawn
    [MessageHandler((ushort)MessageIds.playerRespawn)]
    static void playerRespawn(Message message)
    {
        ushort id = message.GetUShort();
        Vector3 position = message.GetVector3();
        Debug.Log("Respawning player " + id);
        NetworkManager.Singleton.playerList[id].gameObject.GetComponent<PlayerInfo>().Respawn(position);
    }

    //Weapon fire
    [MessageHandler((ushort)MessageIds.weaponFire)]
    static void weaponFire(Message message)
    {
        ushort id = message.GetUShort();
        PlayerNetworking player = NetworkManager.Singleton.playerList[id];
        if (player.IsLocal) return;
        Vector3 start = message.GetVector3();
        Vector3 end = message.GetVector3();
        player.GetComponent<WeaponManager>().CurrentWeapon.VisualizeProjectile(start, end, Color.magenta);
    }

    //Weapon update
    [MessageHandler((ushort)MessageIds.weaponUpdate)]
    static void weaponUpdate(Message message)
    {
        ushort id = message.GetUShort();
        ushort[] equippedWeapons = message.GetUShorts();
        ushort selectedWeaponId = message.GetUShort();
        WeaponManager weaponManager = NetworkManager.Singleton.playerList[id].gameObject.GetComponent<WeaponManager>();
        weaponManager.EquippedWeapons = equippedWeapons;
        weaponManager.SetWeapon(selectedWeaponId, false);
    }
}
