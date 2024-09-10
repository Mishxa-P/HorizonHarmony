using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    public static AudioManager Singleton;
    private List<Sound> fadingSounds = new List<Sound>();

    private void Awake()
    {
        if (Singleton == null) 
        {
            Singleton = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Initialize()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.currentVolume = s.volume;
            s.currentVolume *= GameSettings.globalVolume;
            if (s.isMusic)
            {
                s.currentVolume *= GameSettings.musicVolume;
            }
            else
            {
                s.currentVolume *= GameSettings.soundsVolume;
            }
            s.source.volume = s.currentVolume;

            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    private void Update()
    {
        if (fadingSounds != null && fadingSounds.Count > 0)
        {
            Queue<Sound> temp = new Queue<Sound>();
            int count = fadingSounds.Count;
            for (int i = 0; i < count; i++)
            {
                if (fadingSounds[i].isFadingOut)
                {
                    if (fadingSounds[i].source.volume > 0)
                    {
                        fadingSounds[i].source.volume -= fadingSounds[i].fadePerSecond * Time.deltaTime;
                    }
                    else
                    {
                        fadingSounds[i].isFadingOut = false;
                        Stop(fadingSounds[i].name);
                        temp.Enqueue(fadingSounds[i]);
                    }
                }
                if (fadingSounds[i].isFadingIn)
                {
                    if(fadingSounds[i].source.volume < fadingSounds[i].currentVolume)
                    {
                        fadingSounds[i].source.volume += fadingSounds[i].fadePerSecond * Time.deltaTime;
                    }
                    else
                    {
                        fadingSounds[i].isFadingIn = false;
                        temp.Enqueue(fadingSounds[i]);
                    }
                }
            }
            while(temp.Count > 0)
            {
                fadingSounds.Remove(temp.Dequeue());
            }
        }
    }
    public void UpdateVolumeSettings()
    {
        foreach (Sound s in sounds)
        {
            s.currentVolume = s.volume;
            s.currentVolume *= GameSettings.globalVolume;
            if (s.isMusic)
            {
                s.currentVolume *= GameSettings.musicVolume;
            }
            else
            {
                s.currentVolume *= GameSettings.soundsVolume;
            }
            s.source.volume = s.currentVolume;
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audio clip: " + name + " not found");
            return;
        }
        s.source.volume = s.currentVolume;
        s.source.Play();
    }
    public void FadeIn(string name, float duration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audio clip: " + name + " not found");
            return;
        }
        if (s.isFadingIn)
        {
            Debug.Log("Audio clip: " + name + " is already fading in");
            return;
        }
        s.fadePerSecond = s.currentVolume / duration;
        s.isFadingIn = true;
        s.source.volume = 0.0f;
        s.source.Play();
        fadingSounds.Add(s);
    }

    public void FadeOut(string name, float duration)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audio clip: " + name + " not found");
            return;
        }
        if (s.isFadingOut || !s.source.isPlaying)
        {
            Debug.Log("Audio clip: " + name + " is already fading out or not playing");
            return;
        } 
        s.fadePerSecond = s.currentVolume / duration;
        s.isFadingOut = true;
        fadingSounds.Add(s);
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audio clip: " + name + " not found");
            return;
        }
        s.isFadingIn = false;
        s.isFadingOut = false;
        s.source.Stop();
    }
    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            if (s != null)
            {
                s.isFadingIn = false;
                s.isFadingOut = false;
                s.source.Stop();
            }   
        }
    }
    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audio clip: " + name + " not found");
            return;
        }
        s.source.Pause();
    }
    public void UnPause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Audio clip:  " + name + " not found");
            return;
        }
        s.source.UnPause();
    }
}
