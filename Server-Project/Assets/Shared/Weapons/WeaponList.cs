using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class WeaponList
{
    public static Dictionary<ushort, Weapon_Data> weaponIdToScript = new Dictionary<ushort, Weapon_Data>();

    public static void RefreshDictionary()
    {
        weaponIdToScript.Clear();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(Weapon_Data).Name);  //FindAssets uses tags check documentation for more info
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            Weapon_Data asset = AssetDatabase.LoadAssetAtPath<Weapon_Data>(path);
            weaponIdToScript.Add(asset.weaponId, asset);
        }
    }
}
