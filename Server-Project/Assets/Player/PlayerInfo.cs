using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public static float maxHealth = 100;
    public bool alive = true;

    private ushort playerId;

    void Start()
    {
        health = maxHealth;
        playerId = transform.GetComponent<PlayerNetworking>().Id;
    }

    void Update()
    {
        if(health <= 0 && alive)
        {
            health = 0;
            Death();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerHealthUpdate);
        msg.AddUShort(playerId);
        msg.AddFloat(health);
        NetworkManager.Singleton.Server.SendToAll(msg);
    }

    public void Death()
    {
        alive = false;
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerDeath);
        msg.AddUShort(playerId);
        NetworkManager.Singleton.Server.SendToAll(msg);

        StartCoroutine(TimedActions.StartTimedAction(5/**30*/, () =>
        {
            Respawn(new Vector3(0, 0, 0));
        }));
    }

    public void Respawn(Vector3 spawnPosition)
    {
        health = maxHealth;
        alive = true;
        transform.position = spawnPosition;
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.playerRespawn);
        msg.AddUShort(playerId);
        msg.AddVector3(spawnPosition);
        NetworkManager.Singleton.Server.SendToAll(msg);
    }
}
