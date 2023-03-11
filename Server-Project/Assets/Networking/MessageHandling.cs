using Riptide;
using Riptide.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MessageHandling : MonoBehaviour
{
    #region player
    [MessageHandler((ushort)MessageIds.playerInformation)]
    static void PlayerInformation(ushort fromClientId, Message message)
    {
        //Set info
        ushort id = fromClientId;
        string username = message.GetString();
        Vector3 spawnPos = new Vector3(0f, 2f, 0f);

        //Tell new player to spawn all current ones
        foreach (PlayerNetworking otherPlayer in NetworkManager.Singleton.playerList.Values)
        {
            Message msg = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerJoined);
            msg.AddUShort(otherPlayer.Id);
            msg.AddString(otherPlayer.username);
            msg.AddVector3(otherPlayer.transform.position);

            NetworkManager.Singleton.Server.Send(msg, fromClientId);
        }

        //Spawn player server side
        NetworkManager.SpawnPlayer(id, username, spawnPos);

        //Tell all clients to spawn player
        message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerJoined);
        message.AddUShort(id);
        message.AddString(username);
        message.AddVector3(spawnPos);

        NetworkManager.Singleton.Server.SendToAll(message);
    }
    //Get updated position
    [MessageHandler((ushort)MessageIds.playerTransformUpdate)]
    static void PlayerTransformUpdate(ushort id, Message message)
    {
        Transform transform = NetworkManager.Singleton.playerList[id].transform;
        Vector3 position = message.GetVector3();
        if (position != Vector3.zero)
            transform.position = position;
        Quaternion rotation = message.GetQuaternion();
        if (rotation != Quaternion.identity)
            transform.rotation = rotation;
    }
    #endregion
    #region weapons
    [MessageHandler((ushort)MessageIds.weaponFire)]
    static void WeaponFire(ushort id, Message message)
    {
        PlayerNetworking player = NetworkManager.Singleton.playerList[id];
        player.weapons[player.selectedWeapon].Fire(message.GetVector3());
    }
    [MessageHandler((ushort)MessageIds.weaponUpdate)]
    static void WeaponUpdate(ushort id, Message message)
    {
        PlayerNetworking player = NetworkManager.Singleton.playerList[id];
        ushort[] weaponIds = message.GetUShorts();
        uint selectedWeapon = message.GetUInt();
        player.weapons = new WeaponScript[weaponIds.Length];
        for(int i = 0; i < weaponIds.Length; i++)
        {
            Weapon_Data data = WeaponList.weaponIdToScript[weaponIds[i]];
            GameObject weaponObject = Instantiate(new GameObject(data.weaponName),player.transform.position,player.transform.rotation,player.transform);
            player.weapons[i] = weaponObject.GetComponent<WeaponScript>();
        }
        player.selectedWeapon = selectedWeapon;
    }
    #endregion
}
