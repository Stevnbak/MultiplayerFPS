using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class WeaponScript : MonoBehaviour
{
    [Header("Editor Settings")]
    public Weapon_Data weaponData;
    public BulletScript bullet;

    [Header("Manager Variables")]
    [HideInInspector] public bool isFiring;
    [HideInInspector] public bool stopBurst;
    public float currentAmmo;
    float lastFire = 0;

    void Start()
    {
        name = weaponData.weaponName;
    }

    void Update()
    {
        if (isFiring && Time.time > lastFire + weaponData.fireRate)
        {
            stopBurst = false;
            lastFire = Time.time;
            StartCoroutine(FireWeapon());
            if (!weaponData.auto)
            {
                isFiring = false;
            }
        }
    }

    public IEnumerator FireWeapon()
    {
        for (int burst = 0; burst < weaponData.burstCount; burst++)
        {
            if (stopBurst) {
                stopBurst = false;
                yield break; 
            }
            if (currentAmmo - 1 < 0) continue;
            currentAmmo -= 1;          
            for (int i = 0; i < weaponData.initialShotCount; i++)
            {
                FireProjectile();
            }
            new WaitForSeconds(weaponData.burstDelay);
        }
        yield return null;
    }

    void FireProjectile()
    {
        ///Debug.Log("Fire");
        Vector3 shootingPoint = transform.position + weaponData.shootPoint;
        //Raycast:
        Vector3 direction = Camera.main.transform.forward;
        int layerMask = LayerMask.GetMask("World", "Players");
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, Mathf.Infinity, layerMask))
        {
            Color color;
            if (hit.transform.GetComponent<PlayerInfo>())
            {
                //Object hit was a player...
                ///Debug.Log("Hit player");
                color = Color.green;
            } else
            {
                ///Debug.Log("Hit world object");
                color = Color.blue;
            }
            //Tell server, shots fired:
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.weaponFire);
            Vector3 actualDir = (hit.point - shootingPoint);
            actualDir.Normalize();
            message.AddVector3(actualDir);
            NetworkManager.Singleton.Client.Send(message);

            //Visuals
            Debug.DrawRay(shootingPoint, (hit.point - shootingPoint), color, 5);
            BulletScript temp = Instantiate(bullet.gameObject, shootingPoint, transform.rotation, GameInformation.Singleton.tempParent).GetComponent<BulletScript>();
            temp.Initialize((hit.point - shootingPoint), weaponData.projectileSpeed, hit.point);
        }
        else
        {
            Debug.DrawRay(shootingPoint, direction * 1000, Color.red, 5);
            BulletScript temp = Instantiate(bullet.gameObject, shootingPoint, transform.rotation, GameInformation.Singleton.tempParent).GetComponent<BulletScript>();
            temp.Initialize(direction, weaponData.projectileSpeed, shootingPoint + direction * 1000);
            ///Debug.Log("Did not Hit");
        }
    }
}
