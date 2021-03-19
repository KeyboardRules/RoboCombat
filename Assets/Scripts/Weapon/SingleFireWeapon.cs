using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleFireWeapon : Weapon
{
    float nextToFire;
    bool perfectShoot;
    float timeRecover;
    public override void Fire(Camera weaponCam)
    {
        if (isFiring)
        {
            float recoilX, recoilY;
            currentAmmo--;
            PlayerInfoUI.instance.UpdateText(2, currentAmmo.ToString());
            if (perfectShoot)
            {
                recoilX = recoilY = 0;
                perfectShoot = false;
            }
            else
            {
                recoilX = Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount);
                recoilY = Random.Range(-weaponInfo.recoilAmount, weaponInfo.recoilAmount);
            }
            ani.SetTrigger("Shoot");
            Ray ray = weaponCam.ViewportPointToRay(new Vector3(0.5f+recoilX, 0.5f+recoilY));
            ray.origin = weaponCam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(weaponInfo.damage, PhotonNetwork.LocalPlayer);
                pv.RPC("RPC_Hit", RpcTarget.All, hit.point, hit.normal);
            }
            isFiring = false;
        }
        else 
        {
            if (timeRecover >0)
            {
                timeRecover -= Time.deltaTime;
            }
            else
            {
                perfectShoot = true;
                timeRecover = weaponInfo.recoilRecoverTime;
            }
        }
    }
    public override void GetInput()
    {
        if (isReloading) return;
        if (Input.GetMouseButtonDown(1))
        {
            if (!isAiming)
            {
                ani.SetBool("IsAiming", true);
                isAiming = true;
            }
            else
            {
                ani.SetBool("IsAiming", false);
                isAiming = false;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if(currentAmmo > 0 && Time.time >= nextToFire)
            {
                nextToFire = Time.time + 1f / weaponInfo.fireRate;
                timeRecover = weaponInfo.recoilRecoverTime;
                audi.ReplaySound("Shoot");
                audi.HashTransaction("SoundReplay", "Shoot");
                isFiring = true;
            }
            if (currentAmmo == 0)
            {
                audi.ReplaySound("Empty");
                audi.HashTransaction("SoundReplay", "Empty");
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < weaponInfo.magazine)
        {
            audi.PlaySound("Reload");
            audi.HashTransaction("SoundPlay", "Reload");
            Reload();
        }
    }
    public override void Reload()
    {
        isAiming = false;
        ani.SetBool("IsAiming", false);
        StartCoroutine(Reloading());
    }
}
