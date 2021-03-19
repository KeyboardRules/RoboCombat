using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0, 1f)] public float volumn = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
