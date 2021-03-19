using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFireWeapon : Weapon
{
    float nextToFire;
    bool perfectShoot;
    float timeRecover;
    public override void Fire(Camera weaponCam)
    {
        if (isFiring && Time.time >= nextToFire)
        {
            float recoilX,recoilY;
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
            if (isAiming)
            {
                recoilX = recoilX / 3;
                recoilY = recoilY / 3;
            }
            Ray ray = weaponCam.ViewportPointToRay(new Vector3(0.5f+recoilX, 0.5f+recoilY));
            ray.origin = weaponCam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(weaponInfo.damage,pv.Owner);
                pv.RPC("RPC_Hit", RpcTarget.All, hit.point, hit.normal);
            }
            nextToFire = Time.time + 1f / weaponInfo.fireRate;
        }
        if (!isFiring)
        {
            if (timeRecover > 0)
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
        if (Input.GetMouseButtonDown(0) )
        {
            if(!isFiring && currentAmmo > 0)
            {
                audi.PlaySound("SingleShoot");
                audi.HashTransaction("SoundPlay","SingleShoot");
                ani.SetBool("IsShooting", true);
                isFiring = true;
            }
            if (currentAmmo == 0)
            {
                audi.ReplaySound("Empty");
                audi.HashTransaction("SoundReplay", "Empty");
            }
        }
        if (Input.GetMouseButton(0) && isFiring && currentAmmo > 0)
        {
            audi.PlaySound("Shoot");
            audi.HashTransaction("SoundPlay", "Shoot");
        }
        if (!Input.GetMouseButton(0) && isFiring || currentAmmo == 0 )
        {
            if (isFiring)
            {
                audi.StopSound("Shoot");
                audi.PlaySound("SingleShoot");
                audi.HashTransaction("SoundStop", "Shoot");
                audi.HashTransaction( "SoundShoot", "Play");
                ani.SetBool("IsShooting", false);
            }
            isFiring = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < weaponInfo.magazine)
        {
            audi.StopSound("Shoot");
            audi.PlaySound("Reload");
            audi.HashTransaction("SoundStop","Shoot");
            audi.HashTransaction( "SoundPlay","Reload");
            Reload();
        }
    }

    public override void Reload()
    {
        isFiring = false;
        ani.SetBool("IsShooting", false);
        isAiming = false;
        ani.SetBool("IsAiming", false);
        StartCoroutine(Reloading());
    }
}
