using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using System.IO;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    public float timePrepare;
    public float timeOver;
    public GameState gs;
    int round, playerLeft;
    GameObject localPlayerManager;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["playerLeft"] !=null)
        {
            playerLeft = (int)PhotonNetwork.CurrentRoom.CustomProperties["playerLeft"];
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties["round"] !=null)
        {
            round = (int)PhotonNetwork.CurrentRoom.CustomProperties["round"];
        }
        Hashtable hash = new Hashtable();
        hash.Add("kitName", MusicKitController.instance.GetMusicKitName());
        hash.Add("isDeath", true);
        hash.Add("kitIndex", MusicKitController.instance.GetMusicKitIndex());
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public override void OnCreatedRoom()
    {
        gs = GameState.notInGame;
        round = 0;
        PhotonNetwork.CurrentRoom.MaxPlayers = 8;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1 && SceneManager.GetActiveScene().buildIndex==1 && (gs == GameState.notInGame||gs==GameState.prepare))
        {
            Hashtable hash = new Hashtable();
            hash.Add("nextRound", true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        if ((bool)newPlayer.CustomProperties["isDeath"]==false && gs != GameState.notInGame)
        {
            PlayerLeftOrDeathUpdate();
        }
    }
    void PlayerLeftOrDeathUpdate()
    {
        playerLeft--;
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Hashtable hash = new Hashtable();
            if(playerLeft==1) hash.Add("endRound", true);
            hash.Add("playerLeft", playerLeft);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }
    void RoundPrepare()
    {
        gs = GameState.prepare;
        localPlayerManager.GetComponent<PlayerManager>().Spawn();
        MusicKitController.instance.StartRound();
        ReadyUI.instance.gameObject.SetActive(true);
        ReadyUI.instance.SetReadyText("Ready for the new round...");
        round++;
        playerLeft = PhotonNetwork.CurrentRoom.PlayerCount;
        if (PhotonNetwork.LocalPlayer.IsMasterClient && playerLeft>1)
        {
            Hashtable hash = new Hashtable();
            hash.Add("round", round);
            hash.Add("playerLeft", playerLeft);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            StartCoroutine(Master_PrepareCountdown());
        }
    }
    void RoundPlay()
    {
        gs = GameState.isPlaying;
        ReadyUI.instance.SetReadyText("Round Start!");
    }
    void RoundOver()
    {
        gs = GameState.over;
        
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["isDeath"]==false)
            {
                PlayerMVPUI.instance.SetPlayerMVP(player.NickName,(string)player.CustomProperties["kitName"]);
                MusicKitController.instance.MVPRound((int)player.CustomProperties["kitIndex"]);
                PlayerMVPUI.instance.gameObject.SetActive(true);
                break;
            }
        }
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            StartCoroutine(Master_OverCountdown());
        }
    }
    IEnumerator Master_PrepareCountdown()
    {
        yield return new WaitForSeconds(timePrepare);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Hashtable hash = new Hashtable();
            hash.Add("startRound", true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }
    IEnumerator Master_OverCountdown()
    {
        yield return new WaitForSeconds(timeOver);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Hashtable hash = new Hashtable();
            hash.Add("nextRound", true);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            ReadyUI.instance.SetTimeReady(timePrepare + 2f);
            ReadyUI.instance.gameObject.SetActive(false);
            PlayerMVPUI.instance.SetTimeDisable(timeOver);
            PlayerMVPUI.instance.gameObject.SetActive(false);
            if (PhotonNetwork.LocalPlayer.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount>1)
            {
                Hashtable hash = new Hashtable();
                hash.Add("nextRound", true);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }
            localPlayerManager=PhotonNetwork.Instantiate("GamePlay/PlayerManager", Vector3.zero, Quaternion.identity);
        }
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged["nextRound"] != null)
        {
            RoundPrepare();
        }
        if (propertiesThatChanged["startRound"] != null)
        {
            RoundPlay();
        }
        if (propertiesThatChanged["endRound"] != null)
        {
            RoundOver();
        }
        if (propertiesThatChanged["playerKilled"] != null)
        {
            PlayerLeftOrDeathUpdate();
        }
    }
}
public enum GameState
{
    notInGame,
    prepare,
    isPlaying,
    over,
}
