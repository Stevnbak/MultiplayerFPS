using System.Collections;
using System.Collections.Generic;
using Riptide;
using Riptide.Utils;
using UnityEngine;

public class PlayerNetworking : MonoBehaviour
{
    //Information
    [Header("Client Information")]
    public ushort Id;
    public string username;

    [Header("Player Information")]
    public string state;
    public ushort[] EquippedWeapons;
    public WeaponScript CurrentWeapon;
    private PlayerInfo playerInfo;

    [Header("Settings")]
    public bool position;
    public bool rotation;

    private void Start()
    {
        playerInfo = GetComponent<PlayerInfo>();
    }

    void FixedUpdate()  
    {
        if(!playerInfo.alive)
        {
            transform.position = GameInformation.Singleton.deathPosition;
        }
        //Send position to all clients
        Message message = Message.Create(MessageSendMode.Unreliable, (ushort)MessageIds.playerTransformUpdate);
        message.AddUShort(Id);
        if (position) message.AddVector3(transform.position); else message.AddVector3(Vector3.zero);
        if (rotation) message.AddQuaternion(transform.rotation); else message.AddQuaternion(Quaternion.identity);
        NetworkManager.Singleton.Server.SendToAll(message);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.playerList.Remove(Id);
    }
}
