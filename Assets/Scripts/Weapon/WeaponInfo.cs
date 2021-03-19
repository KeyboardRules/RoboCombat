using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Info")]
public class WeaponInfo : ScriptableObject
{
    public string itemName;
    public float damage;
    public float fireRate;
    public float reloadTime;
    public float magazine;
    public float recoilAmount;
    public float recoilRecoverTime;
}
