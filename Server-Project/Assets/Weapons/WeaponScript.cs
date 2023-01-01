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

    void Start()
    {
        name = weaponData.weaponName;
    }

    public void Fire(Vector3 direction)
    {
        ///Debug.Log("Fire");
        Vector3 shootingPoint = transform.position + weaponData.shootPoint;
        //Raycast:
        int layerMask = LayerMask.GetMask("World", "Players");
        RaycastHit hit;
        if (Physics.Raycast(shootingPoint, direction, out hit, Mathf.Infinity, layerMask))
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

            //Visuals
            Debug.DrawRay(shootingPoint, (hit.point - shootingPoint), color, 5);
        }
        else
        {
            Debug.DrawRay(shootingPoint, direction * 1000, Color.red, 5);
            ///Debug.Log("Did not Hit");
        }
    }
}
