using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicKit 
{
    public string nameKit;
    public Sound menu,startRound,mvpPlayer;
    
    public void MenuPlay()
    {
        startRound.source.Stop();
        mvpPlayer.source.Stop();
        menu.source.Play();
    }
    public void MenuStop()
    {
        menu.source.Stop();
    }
    public void StartRoundPlay()
    {
        mvpPlayer.source.Stop();
        startRound.source.Play();
    }
    public void MVPPlay()
    {
        startRound.source.Stop();
        mvpPlayer.source.Play();
    }
}
