using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    SpawnPoint[] spawnPoints;
    private void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPoint>();
    }
    public Transform GetSpawnPoint()
    {
        int index = 0;
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer) break;
            index++;
        }
        return spawnPoints[index].transform;
    }
}
