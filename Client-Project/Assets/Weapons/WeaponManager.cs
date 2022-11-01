using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponScript[] EqippedWeapons;
    public int selectedWeapon = 0;

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

    void Start()
    {
        input = GetComponent<InputManager>();
        StartCoroutine(Reload(true));
    }

    void Update()
    {
        //Update ammo count
        currentAmmo = EqippedWeapons[selectedWeapon].currentAmmo;

        //Inputs
        Fire(input.shoot);
        ADS(input.ads);
        if(input.reload)
        {
            input.reload = false;
            StartCoroutine(Reload());
        }
        StartCoroutine(SwapWeapon(input.weapon));
    }

    void ADS(bool state)
    {
        //Debug.Log("ADS: " + state);
    }

    IEnumerator SwapWeapon(int newWeapon)
    {
        if (newWeapon == selectedWeapon || swappingInProgress) yield break;
        Debug.Log("Switching to weapon: " + newWeapon);
        cancelReload();
        Fire(false);
        EqippedWeapons[selectedWeapon].stopBurst = true;
        swappingInProgress = true;
        //Start unequip animation
        yield return new WaitForSeconds(EqippedWeapons[selectedWeapon].weaponData.unequipTime);
        if (newWeapon > EqippedWeapons.Length - 1) newWeapon = 0;
        if (newWeapon < 0) newWeapon = EqippedWeapons.Length - 1;
        //Start equip animation
        yield return new WaitForSeconds(EqippedWeapons[newWeapon].weaponData.equipTime);
        selectedWeapon = newWeapon;
        swappingInProgress = false;
        yield return null;
    }
    
    void Fire(bool state)
    {
        //Debug.Log("Fire: " + state);
        if (state)
        {
            if (swappingInProgress) return;
            EqippedWeapons[selectedWeapon].isFiring = true;
            cancelReload();
        } else
        {
            EqippedWeapons[selectedWeapon].isFiring = false;
        }
    }

    IEnumerator Reload(bool skipTime = false)
    {
        Debug.Log("Reloading");
        if (swappingInProgress || isReloading) yield break;
        EqippedWeapons[selectedWeapon].stopBurst = true;
        isReloading = true;
        currentAmmo = EqippedWeapons[selectedWeapon].currentAmmo;
        Fire(false);
        if (!skipTime) {
            //Start animation
            yield return new WaitForSeconds(EqippedWeapons[selectedWeapon].weaponData.reloadTime); 
        }
        if(!isReloading) yield break;
        float magSize = EqippedWeapons[selectedWeapon].weaponData.magSize;
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
        EqippedWeapons[selectedWeapon].currentAmmo = currentAmmo;
        isReloading = false;
        yield return null;
    }

    void cancelReload()
    {
        isReloading = false;
        //Stop reload animation
    }
}
