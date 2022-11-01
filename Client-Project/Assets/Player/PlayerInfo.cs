using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float health;
    public static float maxHealth = 100;

    void Start()
    {
        health = maxHealth;
    }

    public void updateHealth(float newHealth)
    {
        health = newHealth;
    }

}
