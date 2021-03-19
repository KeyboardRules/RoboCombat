using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public static ConnectionManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        OpenScene(0);
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Launcher.Instance.Error(message);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        OpenScene(0);
    }
    void OpenScene(int index)
    {
        if (SceneManager.GetActiveScene().buildIndex == index)
            return;
        SceneManager.LoadScene(index);
    }
}
