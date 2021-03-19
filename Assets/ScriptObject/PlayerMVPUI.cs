using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMVPUI : MonoBehaviour
{
    public static PlayerMVPUI instance;
    [SerializeField] TextMeshProUGUI playerMVP, playerMusicKit;
    float timeMVP;
    private void Awake()
    {
        instance = this;
    }
    public void SetTimeDisable(float time)
    {
        timeMVP = time;
    }
    public void SetPlayerMVP(string playerName, string? playerMusicKit)
    {
        playerMVP.text = playerName;
        this.playerMusicKit.text = playerMusicKit;
    }
    private void OnEnable()
    {
        StartCoroutine(DisableAfter());
    }
    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(timeMVP);
        gameObject.SetActive(false);
    }
}
