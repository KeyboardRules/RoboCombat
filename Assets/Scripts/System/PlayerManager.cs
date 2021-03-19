using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] ParticleSystem explosion;
    int kill, death;
    bool isDeath;
    PhotonView pv;
    GameObject controller;
    // Start is called before the first frame update
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        PlayerInfoUI.instance.UpdateText(3, kill.ToString());
        PlayerInfoUI.instance.UpdateText(4, death.ToString());
    }
    public Player Owner()
    {
        return pv.Owner;
    }
    public void PlayerAddKill(int kill)
    {
        this.kill +=kill;
        if (pv.IsMine)
        {
            PlayerInfoUI.instance.UpdateText(3, this.kill.ToString());
        }
    }
    public void Spawn()
    {
        isDeath = false;
        if (!pv.IsMine) return;
        if (controller != null) PhotonNetwork.Destroy(controller);
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("Player", "Player"), spawnPoint.transform.position, spawnPoint.transform.rotation, 0, new object[] { pv.ViewID });
        Hashtable hash = new Hashtable();
        hash.Add("isDeath", isDeath);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void Die(Player damageDealer)
    {
        PlayerInfoUI.instance.UpdateText(4, death.ToString());
        pv.RPC("RPC_Die", RpcTarget.All, controller.transform.position,damageDealer);
        PhotonNetwork.Destroy(controller);
    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);
    }
    [PunRPC]
    void RPC_Die(Vector3 position,Player damageDealer)
    {
        isDeath = true;
        death++;
        PlayerManager[] playerManagers = FindObjectsOfType<PlayerManager>();
        foreach(PlayerManager playerManager in playerManagers)
        {
            if (playerManager.Owner()==damageDealer)
            {
                playerManager.PlayerAddKill(1);
            }
        }
        if (pv.IsMine)
        {
            PlayerInfoUI.instance.UpdateText(4, death.ToString());
            Hashtable hash = new Hashtable();
            hash.Add("isDeath", isDeath);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            hash = new Hashtable();
            hash.Add("playerKilled", true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
        RoomInfoManager.instance.NewMessageContainer(damageDealer.NickName + " Kill " + pv.Owner.NickName);
        Instantiate(explosion, position,Quaternion.identity);
    }
}
