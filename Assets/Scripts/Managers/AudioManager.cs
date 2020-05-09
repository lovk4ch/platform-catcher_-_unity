using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip clip;

    public string name;

    [Range(0f, 1f)]
    public float spatialBlend = 1;
    [Range(0f, 1f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;
    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;

    public Sound(AudioSource source, AudioClip clip, float spatialBlend, float volume, float pitch, bool loop, bool playOnAwake)
    {
        this.source = source;
        this.source.clip = clip;

        this.source.spatialBlend = spatialBlend;
        this.source.volume = volume;
        this.source.pitch = pitch;
        this.source.loop = loop;
        this.source.playOnAwake = playOnAwake;
    }
}

public class AudioManager : Manager<AudioManager>
{
    [SerializeField]
    private Sound[] sounds = null;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.spatialBlend = s.spatialBlend;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    private AudioSource GetSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        return s.source;
    }

    public void Play(string name)
    {
        GetSound(name).Play();
    }
}