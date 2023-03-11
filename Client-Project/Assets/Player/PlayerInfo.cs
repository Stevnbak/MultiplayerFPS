using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public static float maxHealth = 100;
    public bool alive = true;

    void Start()
    {
        health = maxHealth;
        UIManager.Singleton.DeathScreen.SetActive(false);
    }

    public void UpdateHealth(float newHealth)
    {
        health = newHealth;
    }

    public void Death()
    {
        health = 0;
        alive = false;
        UIManager.Singleton.DeathScreen.SetActive(true);
    }

    public void Respawn(Vector3 spawnPosition)
    {
        health = maxHealth;
        alive = true;
        UIManager.Singleton.DeathScreen.SetActive(false);
        transform.position = spawnPosition;
    }
}
