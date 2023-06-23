using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

public class WeaponScript : MonoBehaviour
{
    [Header("Editor Settings")]
    public Weapon_Data weaponData;

    private PlayerNetworking playerNetworking;

    void Start()
    {
        name = weaponData.weaponName;
        playerNetworking = GetComponent<PlayerNetworking>();
    }

    public void Fire(Vector3 direction, ushort playerId)
    {
        Debug.Log("Fire");
        Vector3 shootingPoint = transform.position + Weapon_Data.gunPosition + weaponData.shootPoint;
        //Raycast:
        int layerMask = LayerMask.GetMask("World", "Players");
        RaycastHit hit;
        Color color;
        Vector3 hitPoint;
        if (Physics.Raycast(shootingPoint, direction, out hit, Mathf.Infinity, layerMask))
        {
            hitPoint = hit.point;
            PlayerInfo playerHit = hit.transform.GetComponent<PlayerInfo>() ?? hit.transform.GetComponentInParent<PlayerInfo>() ?? hit.transform.GetComponentInChildren<PlayerInfo>();
            if (playerHit)
            {
                //Object hit was a player...
                Debug.Log("Hit player");
                color = Color.green;
                playerHit.TakeDamage(weaponData.damage);
            }
            else
            {
                Debug.Log("Hit world object");
                color = Color.blue;
            }
        }
        else
        {
            color = Color.red;
            hitPoint = shootingPoint + direction * 1000;
            Debug.Log("Did not Hit");
        }
        //Visuals
        Debug.DrawRay(shootingPoint, hitPoint - shootingPoint, color, 5);

        //Tell all clients too
        Message message = Message.Create(MessageSendMode.Reliable, MessageIds.weaponFire);
        Debug.Log("Id: " + playerId);
        message.AddUShort(playerId);
        message.AddVector3(shootingPoint);
        message.AddVector3(hitPoint);
       ServerManager.Singleton.GetServer(playerNetworking.serverId).Server.SendToAll(message);
    }
}
