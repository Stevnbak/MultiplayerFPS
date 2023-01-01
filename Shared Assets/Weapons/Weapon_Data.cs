using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon_Data", order = 1)]
public class Weapon_Data : ScriptableObject
{
    [Header("Basic Information")]
    public string weaponName;
    public ushort weaponId;
    public string weaponCategory, ammoType = "Normal";

    [Header("Basic Stats")]
    public Vector3 shootPoint;
    public float damage, fireRate, reloadTime, magSize, projectileSpeed, headshotMultiplier = 2, equipTime = 0.5f, unequipTime = 0.1f;
    public bool auto = true;
    
    [Header("MultiShot")]
    public int burstCount = 1;
    public float burstDelay;
    public int initialShotCount = 1;

    [Header("Falloff")]
    public float falloffMultiplier = 1;
    public float falloffEnd,  falloffStart;

}