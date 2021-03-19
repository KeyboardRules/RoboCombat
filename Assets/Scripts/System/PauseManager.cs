using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    public bool isPause;
    private void Awake()
    {
        instance = this;
        ChangePauseStatus();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.Instance.ResponsiveMenu("pause");
            ChangePauseStatus();
        }
    }
    public void ReturnButton()
    {
        MenuManager.Instance.ResponsiveMenu("pause");
        ChangePauseStatus();
    }
    public void LeaveButton()
    {
        PhotonNetwork.LeaveRoom();
        Destroy(RoomManager.Instance.gameObject);
    }
    void ChangePauseStatus()
    {
        isPause = !isPause;
        if (isPause)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
