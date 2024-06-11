using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0.0f, 1.0f)]
    public float volume;
    [Range(1.0f, 3.0f)]
    public float pitch;
    public bool loop;
    public bool isMusic;

    public float currentVolume;

    [HideInInspector]
    public bool isFadingIn;
    [HideInInspector]
    public bool isFadingOut;
    [HideInInspector]
    public float fadePerSecond;
    [HideInInspector]
    public AudioSource source;
}
