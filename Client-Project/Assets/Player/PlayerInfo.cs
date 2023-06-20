using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public static float maxHealth = 100;
    public bool alive = true;

    PlayerNetworking playerNetworking;

    void Start()
    {
        playerNetworking = GetComponent<PlayerNetworking>();
        health = maxHealth;
        if (playerNetworking.IsLocal) UIManager.Singleton.DeathScreen.SetActive(false);
    }

    public void UpdateHealth(float newHealth)
    {
        health = newHealth;
    }

    public void Death()
    {
        health = 0;
        alive = false;
        if(playerNetworking.IsLocal) UIManager.Singleton.DeathScreen.SetActive(true);
    }

    public void Respawn(Vector3 spawnPosition)
    {
        Debug.Log("Respawning player " + playerNetworking.name);
        health = maxHealth;
        alive = true;
        if (playerNetworking.IsLocal) UIManager.Singleton.DeathScreen.SetActive(false);
        transform.position = spawnPosition;
    }
}
