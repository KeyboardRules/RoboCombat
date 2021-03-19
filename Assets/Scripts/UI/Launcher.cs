using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startRoomButton;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
            Instance.Error("Disconnected,Check the internet pls");
    }
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        roomNameInputField.text = null;
        MenuManager.Instance.OpenMenu("loading");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        PhotonNetwork.NickName = playerNameInputField.text;
        MenuManager.Instance.OpenMenu("room");

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(players[i]);
        }
        startRoomButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public void StartRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LoadLevel(1);
    }
    public void LeaveRoom()
    {
        MenuManager.Instance.OpenMenu("loading");
        PhotonNetwork.LeaveRoom();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OkError()
    {
        if(!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }
    public void ChangeMusicKit(int val)
    {
        MusicKitController.instance.SelectKit(val);
    }
    public void Error(string message)
    {
        errorText.text = "Failed To Create Room: " + message;
        MenuManager.Instance.OpenMenu("error");
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startRoomButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
        foreach (Transform trans in playerListContent)
        {
            Destroy(trans.gameObject);
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomInfos)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomInfos.Count; i++)
        {
            if (roomInfos[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomInfos[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }
}
