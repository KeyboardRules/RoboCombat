using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;

public class RoomInfoManager : MonoBehaviourPunCallbacks
{
    public static RoomInfoManager instance;
    [SerializeField] TextMeshProUGUI round, playerLeft;
    [SerializeField] GameObject scoreMessageContainer;
    [SerializeField] TextMeshProUGUI scoreKill;

    PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        instance = this;
    }
    public void NewMessageContainer(string message)
    {
        TextMeshProUGUI text = (TextMeshProUGUI)Instantiate(scoreKill, scoreMessageContainer.transform);
        text.SetText(message);
        Destroy(text.gameObject, 3f);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        NewMessageContainer(newPlayer.NickName+" has joined the game.");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        NewMessageContainer(otherPlayer.NickName + " has left the game.");
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties["playerLeft"] != null)
        {
            int playerLeft = (int)PhotonNetwork.CurrentRoom.CustomProperties["playerLeft"];
            this.playerLeft.text = "Player left: " + playerLeft.ToString();
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties["round"] != null)
        {
            int round = (int)PhotonNetwork.CurrentRoom.CustomProperties["round"];
            this.round.text = "Round: " + round.ToString();
        }
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged["playerLeft"] != null)
        {
            int playerLeft= (int)propertiesThatChanged["playerLeft"];
            this.playerLeft.text ="Player left: "+ playerLeft.ToString();
        }
        if(propertiesThatChanged["round"] != null)
        {
            int round= (int)propertiesThatChanged["round"];
            this.round.text ="Round: "+ round.ToString();
        }
    }
}
