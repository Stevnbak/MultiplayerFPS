using Riptide;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponManager : MonoBehaviour
{
    public static uint MaxWeaponCount = 2;
    [Header("Weapons")]
    public ushort[] EquippedWeapons;
    public WeaponScript CurrentWeapon;

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
        EquippedWeapons = new ushort[MaxWeaponCount];
        for (int i = 0; i < MaxWeaponCount; i++) EquippedWeapons[i] = 0;
        SetWeapon(EquippedWeapons[0]);
        StartCoroutine(Reload(true));
    }

    void Update()
    {
        //Update ammo count
        currentAmmo = CurrentWeapon.currentAmmo;

        //Inputs
        Fire(input.shoot);
        ADS(input.ads);
        if(input.reload)
        {
            input.reload = false;
            StartCoroutine(Reload());
        }
        if (EquippedWeapons[input.weapon] != CurrentWeapon.weaponData.weaponId && !swappingInProgress)
        {
            StartCoroutine(SwapWeapon((uint)input.weapon));
        }
    }

    void ADS(bool state)
    {
        //Debug.Log("ADS: " + state);
    }

    IEnumerator SwapWeapon(uint newWeapon)
    {
        Debug.Log("Switching to weapon: " + newWeapon);
        //Get ready for weapon switch
        cancelReload();
        Fire(false);
        CurrentWeapon.stopBurst = true;
        swappingInProgress = true;
        //Unequip old weapon
        ///Start unequip animation
        yield return new WaitForSeconds(CurrentWeapon.weaponData.unequipTime);

        //Equip new weapon
        SetWeapon(EquippedWeapons[newWeapon]);
        ///Start equip animation
        yield return new WaitForSeconds(CurrentWeapon.weaponData.equipTime);
        //Stop swapping
        swappingInProgress = false;
        yield return null;
    }

    public void SetWeapon(ushort weaponId, bool tellServer = true)
    {
        Debug.Log("Setting weapon to " + weaponId);
        if(CurrentWeapon != null)
        {
            Destroy(CurrentWeapon.gameObject);
            CurrentWeapon = null;
        }
        WeaponScript weapon = GameInformation.Singleton.WeaponPrefabList.Find((a) => a.weaponData.weaponId == weaponId);
        GameObject weaponObject = Instantiate(weapon.gameObject, transform.position + Weapon_Data.gunPosition, weapon.transform.rotation, transform);
        CurrentWeapon = weaponObject.GetComponent<WeaponScript>();
        //Send server new weapon info
        if (playerNetworking.IsLocal && tellServer)
        {
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)MessageIds.weaponUpdate);
            message.AddUShorts(EquippedWeapons);
            message.AddUShort(CurrentWeapon.weaponData.weaponId);
            NetworkManager.Singleton.Client.Send(message);
        }
    }
    
    void Fire(bool state)
    {
        //Debug.Log("Fire: " + state);
        if (state)
        {
            if (swappingInProgress) return;
            CurrentWeapon.isFiring = true;
            cancelReload();
        } else
        {
            CurrentWeapon.isFiring = false;
        }
    }

    IEnumerator Reload(bool skipTime = false)
    {
        Debug.Log("Reloading");
        if (swappingInProgress || isReloading) yield break;
        CurrentWeapon.stopBurst = true;
        isReloading = true;
        currentAmmo = CurrentWeapon.currentAmmo;
        Fire(false);
        if (!skipTime) {
            //Start animation
            yield return new WaitForSeconds(CurrentWeapon.weaponData.reloadTime); 
        }
        if(!isReloading) yield break;
        float magSize = CurrentWeapon.weaponData.magSize;
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
        CurrentWeapon.currentAmmo = currentAmmo;
        isReloading = false;
        yield return null;
    }

    void cancelReload()
    {
        isReloading = false;
        //Stop reload animation
    }
}
