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
    public Transform shootingPoint;

    void Start()
    {
        name = weaponData.weaponName;
    }

    public void Fire(Vector3 direction)
    {
        ///Debug.Log("Fire");
        //Raycast:
        int layerMask = LayerMask.GetMask("World", "Players");
        RaycastHit hit;
        if (Physics.Raycast(shootingPoint.position, direction, out hit, Mathf.Infinity, layerMask))
        {
            Color color;
            if (hit.transform.GetComponent<PlayerInfo>())
            {
                //Object hit was a player...
                ///Debug.Log("Hit player");
                color = Color.green;
            }
            else
            {
                ///Debug.Log("Hit world object");
                color = Color.blue;
            }
            //Tell server, shots fired:
            /**Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.weaponFire);
            message.AddVector3(direction);
            NetworkManager.Singleton.Client.Send(message);*/

            //Visuals
            Debug.DrawRay(shootingPoint.position, (hit.point - shootingPoint.position), color, 5);
        }
        else
        {
            Debug.DrawRay(shootingPoint.position, direction * 1000, Color.red, 5);
            ///Debug.Log("Did not Hit");
        }
    }
}
