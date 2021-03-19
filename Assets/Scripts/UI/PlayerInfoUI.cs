using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoUI : MonoBehaviour
{
    public static PlayerInfoUI instance;
    [SerializeField]TextMeshProUGUI playerHealth, playerAmmo,playerKill,playerDeath;
    private void Awake()
    {
        instance = this;
    }
    public void UpdateText(int number,string value)
    {
        switch (number)
        {
            case (1):
                playerHealth.text = value;
                break;
            case (2):
                playerAmmo.text = value;
                break;
            case (3):
                playerKill.text ="Kill: "+ value;
                break;
            case (4):
                playerDeath.text = "Death: " + value;
                break;
        }
    }
}
