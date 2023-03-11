using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public static float maxHealth = 100;
    void Start()
    {
        health = maxHealth;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerHealthUpdate);
        msg.AddUShort(transform.GetComponent<PlayerNetworking>().Id);
        msg.AddFloat(health);
        NetworkManager.Singleton.Server.SendToAll(msg);
    }
}
