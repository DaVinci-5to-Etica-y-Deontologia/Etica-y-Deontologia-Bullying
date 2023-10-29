using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;
    public AudioSource source;
    
    [Range(0, 1)]
    public float volume;
    [Range(-1, 3)]
    public float pitch;

    public bool playOnAwake, loop;

    public AudioMixerGroup audioMixerGroup;
}
