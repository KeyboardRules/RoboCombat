using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class AudioController : MonoBehaviourPunCallbacks
{
    public Sound[] sounds;
    PhotonView pv;
    public void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volumn;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;
            sound.source.spatialBlend = 1;
            sound.source.rolloffMode = AudioRolloffMode.Linear;
            sound.source.minDistance = 0;
            sound.source.maxDistance = 80;
        }
        pv = GetComponent<PhotonView>();
    }
    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.name == name);
        if (s == null || s.source==null)
            return;
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
    }
    public void ReplaySound(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.name == name);
        if (s == null || s.source == null)
            return;
        if (s.source.isPlaying)
        {
            s.source.Stop();
        }
        s.source.Play();
    }
    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sounds => sounds.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }
    public void HashTransaction(string key, object o)
    {
        Hashtable hash = new Hashtable();
        hash.Add(key, o);
        if(PhotonNetwork.NetworkClientState!=ClientState.Leaving) PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void OnDestroy()
    {
        foreach(Sound s in sounds)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == pv.Owner)
        {
            if (changedProps["SoundPlay"] != null)
            {
                PlaySound((string)changedProps["SoundPlay"]);
            }
            if (changedProps["SoundReplay"] != null)
            {
                ReplaySound((string)changedProps["SoundReplay"]);
            }
            if (changedProps["SoundStop"] != null)
            {
                StopSound((string)changedProps["SoundStop"]);

            }
        } 
    }
}
