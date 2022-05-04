using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TestProject1;

[CreateAssetMenu(fileName ="MeleeWeapon", menuName ="Data/Items/MeleeWeapon",order = 1)]
public class MeleeWeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponImage;
    [Range(1, 4)] public int weaponRarity;
    [Range(1,100)] public int weaponDamage;
    [Range(1,500)] public int weaponDurability;
}
