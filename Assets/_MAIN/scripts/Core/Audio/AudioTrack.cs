using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTrack
{
    private const string TRACK_NAME_FORMAT = "Track - {0}";
    public string name { get; private set; }
    public string path { get; private set; }

    public GameObject root => source.gameObject;

    private AudioChannel channel;
    private AudioSource source;
    public bool loop => source.loop;
    public float volumeCap { get; private set; }
    public bool isPlaying => source.isPlaying;

    public float volume { get { return source.volume; } set { source.volume = value; } }

    public AudioTrack(AudioClip clip, bool loop, float startingVolume, float volumeCap, AudioChannel channel, AudioMixerGroup mixer, string filePath)
    {
        name = clip.name;
        this.channel = channel;
        this.volumeCap = volumeCap;
        path = filePath;

        source = CreateSource();
        source.clip = clip;
        source.loop = loop;
        source.volume = startingVolume;
        source.outputAudioMixerGroup = mixer;
    }

    private AudioSource CreateSource()
    {
        GameObject go = new GameObject(string.Format(TRACK_NAME_FORMAT, name));
        go.transform.SetParent(channel.trackContainer);
        AudioSource source = go.AddComponent<AudioSource>();

        return source;
    }

    public void PlayDelayed(float delay)
    {
        source.PlayDelayed(delay);
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
    
}
