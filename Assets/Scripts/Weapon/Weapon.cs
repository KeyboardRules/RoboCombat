using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public abstract class Weapon : MonoBehaviourPunCallbacks
{
    public WeaponInfo weaponInfo;
    public GameObject bulletImapctPrefab;
    public float currentAmmo;
    public Texture2D crosshair;

    public bool isReloading;
    public bool isFiring;
    public bool isDrawing;
    public bool isAiming;
    public Animator ani;
    public AudioController audi;
    public PhotonView pv;

    public abstract void Fire(Camera weaponCam);
    public abstract void Reload();
    public abstract void GetInput();
    void Awake()
    {
        ani = GetComponent<Animator>();
        audi = GetComponent<AudioController>();
        currentAmmo = weaponInfo.magazine;
        pv = GetComponent<PhotonView>();
    }
    public bool IsIndexState()
    {
        if (!isReloading && !isFiring && !isDrawing && !isAiming) return true;
        return false;
    }
    public IEnumerator Reloading()
    {
        isReloading = true;
        ani.SetBool("IsReloading", true);
        yield return new WaitForSeconds(weaponInfo.reloadTime-.05f);
        ani.SetBool("IsReloading", false);
        yield return new WaitForSeconds(.05f);
        currentAmmo = weaponInfo.magazine;
        PlayerInfoUI.instance.UpdateText(2, currentAmmo.ToString());
        isReloading = false;
    }
    IEnumerator Drawing()
    {
        isDrawing = true;
        yield return new WaitForSeconds(.2f);
        isDrawing = false;
    }
    public override void OnEnable()
    {
        audi.PlaySound("Draw");
        PlayerInfoUI.instance.UpdateText(2,currentAmmo.ToString());
        isFiring = false;
        isAiming = false;
        isReloading = false;
        StartCoroutine(Drawing());
    }
    private void OnDestroy()
    {
        audi.StopSound("IsShooting");
    }
    private void OnGUI()
    {
        if (!isAiming && pv.IsMine)
        {
            float xMin = (Screen.width / 2) - ((crosshair.width) / 2);
            float yMin = (Screen.height / 2) - ((crosshair.height) / 2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width , crosshair.height ), crosshair);
        }
    }
    [PunRPC]
    public void RPC_Hit(Vector3 hitPositions, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPositions, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImapctPrefab, hitPositions + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImapctPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
