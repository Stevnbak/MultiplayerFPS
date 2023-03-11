using Riptide;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponScript[] EquippedWeapons;
    public uint selectedWeapon = 0;

    [Header("Position")]
    public Transform equippedPosition;
    public Transform unequippedPosition;
    public Transform adsPosition;

    [Header("Information")]
    public float totalAmmo = 1000;
    public float currentAmmo;


    //Private
    bool swappingInProgress = false;
    //bool lastFiringState = false;
    private bool isReloading = false;
    private InputManager input;
    private PlayerInfo playerInfo;
    private PlayerNetworking playerNetworking;

    void Start()
    {
        input = GetComponent<InputManager>();
        playerInfo = GetComponent<PlayerInfo>();
        playerNetworking = GetComponent<PlayerNetworking>();
        StartCoroutine(Reload(true));
    }

    void Update()
    {
        //Update ammo count
        currentAmmo = EquippedWeapons[selectedWeapon].currentAmmo;

        //Inputs
        Fire(input.shoot);
        ADS(input.ads);
        if(input.reload)
        {
            input.reload = false;
            StartCoroutine(Reload());
        }
        StartCoroutine(SwapWeapon((uint) input.weapon));
    }

    void ADS(bool state)
    {
        //Debug.Log("ADS: " + state);
    }

    IEnumerator SwapWeapon(uint newWeapon)
    {
        if (newWeapon == selectedWeapon || swappingInProgress) yield break;
        Debug.Log("Switching to weapon: " + newWeapon);
        cancelReload();
        Fire(false);
        EquippedWeapons[selectedWeapon].stopBurst = true;
        swappingInProgress = true;
        //Start unequip animation
        yield return new WaitForSeconds(EquippedWeapons[selectedWeapon].weaponData.unequipTime);
        if (newWeapon > EquippedWeapons.Length - 1) newWeapon = 0;
        if (newWeapon < 0) newWeapon = (uint) EquippedWeapons.Length - 1;
        //Start equip animation
        yield return new WaitForSeconds(EquippedWeapons[newWeapon].weaponData.equipTime);
        selectedWeapon = newWeapon;
        swappingInProgress = false;
        //Send server new weapon info
        if (playerNetworking.IsLocal)
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.weaponUpdate);
            ushort[] weapons = new ushort[EquippedWeapons.Length];
            for(int i = 0; i < weapons.Length; i++)
            {
                weapons[i] = EquippedWeapons[i].weaponData.weaponId;
            }
            message.AddUShorts(weapons);
            message.AddUInt(selectedWeapon);
            NetworkManager.Singleton.Client.Send(message);
        }
        yield return null;
    }
    
    void Fire(bool state)
    {
        //Debug.Log("Fire: " + state);
        if (state)
        {
            if (swappingInProgress) return;
            EquippedWeapons[selectedWeapon].isFiring = true;
            cancelReload();
        } else
        {
            EquippedWeapons[selectedWeapon].isFiring = false;
        }
    }

    IEnumerator Reload(bool skipTime = false)
    {
        Debug.Log("Reloading");
        if (swappingInProgress || isReloading) yield break;
        EquippedWeapons[selectedWeapon].stopBurst = true;
        isReloading = true;
        currentAmmo = EquippedWeapons[selectedWeapon].currentAmmo;
        Fire(false);
        if (!skipTime) {
            //Start animation
            yield return new WaitForSeconds(EquippedWeapons[selectedWeapon].weaponData.reloadTime); 
        }
        if(!isReloading) yield break;
        float magSize = EquippedWeapons[selectedWeapon].weaponData.magSize;
        totalAmmo -= magSize;
        if (totalAmmo < 0)
        {
            currentAmmo += totalAmmo + magSize;
            totalAmmo = 0;
        }
        else
        {
            currentAmmo = magSize;
        }
        EquippedWeapons[selectedWeapon].currentAmmo = currentAmmo;
        isReloading = false;
        yield return null;
    }

    void cancelReload()
    {
        isReloading = false;
        //Stop reload animation
    }
}
