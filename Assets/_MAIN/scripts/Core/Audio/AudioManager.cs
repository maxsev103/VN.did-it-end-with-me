using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const string MUSIC_VOLUME_PARAMETER_NAME = "MusicVolume";
    public const string SFX_VOLUME_PARAMETER_NAME = "SFXVolume";
    public const string VOICE_VOLUME_PARAMETER_NAME = "VoiceVolume";

    private const string SFX_PARENT_NAME = "SFX";
    public static readonly char[] SFX_NAME_FORMAT_CONTAINERS = new char[] { '[', ']' };
    private static string SFX_NAME_FORMAT = $"SFX - {SFX_NAME_FORMAT_CONTAINERS[0]}" + "{0}" + $"{SFX_NAME_FORMAT_CONTAINERS[1]}";
    public const float TRACK_TRANSITION_SPEED = 1;

    public static AudioManager instance { get; private set; }

    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup voiceMixer;

    public AnimationCurve audioFallOffCurve;

    private Transform sfxRoot;

    public AudioSource[] allSFX => sfxRoot.GetComponentsInChildren<AudioSource>();

    public void Awake()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists within resources.");
            return null;
        }

        return PlaySoundEffect(clip, filePath, mixer, volume, pitch, loop);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, string filePath = "", AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        string fileName = clip.name;
        if (filePath != string.Empty)
        {
            fileName = filePath;
        }

        AudioSource sfxSource = new GameObject(string.Format(SFX_NAME_FORMAT, fileName)).AddComponent<AudioSource>();
        sfxSource.transform.SetParent(sfxRoot);
        sfxSource.transform.position = sfxRoot.position;

        sfxSource.clip = clip;

        if (mixer == null)
            mixer = sfxMixer;

        sfxSource.outputAudioMixerGroup = mixer;
        sfxSource.volume = volume;
        sfxSource.spatialBlend = 0;
        sfxSource.pitch = pitch;
        sfxSource.loop = loop;

        if (!loop)
            Destroy(sfxSource.gameObject, (clip.length / pitch) + 1);

        sfxSource.Play();

        return sfxSource;
    }

    public AudioSource PlayVoice(string filePath, float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(filePath, voiceMixer, volume, pitch, loop);
    }

    public AudioSource PlayVoice(AudioClip clip, string filePath = "", float volume = 1, float pitch = 1, bool loop = false)
    {
        return PlaySoundEffect(clip, filePath, voiceMixer, volume, pitch, loop);
    }

    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            if (source.clip.name.ToLower() == soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public bool IsPlayingSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources)
        {
            if (source.clip.name.ToLower() == soundName)
            {
                return true;
            }
        }

        return false;
    }

    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists within resources.");
            return null;
        }

        return PlayTrack(clip, filePath, channel, loop, startingVolume, volumeCap);
    }
    public AudioTrack PlayTrack(AudioClip clip, string filePath = "", int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1)
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfNotExisting: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, filePath);
        return track;
    }

    public void StopTrack(int channelIndex)
    {
        AudioChannel c = TryGetChannel(channelIndex, createIfNotExisting: false);
        if (c == null)
            return;

        c.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach (var channel in channels.Values)
        {
            if (channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    public void StopAllTracks()
    {
        foreach (AudioChannel channel in channels.Values)
        {
            channel.StopTrack();
        }
    }

    public void StopAllSoundEffects()
    {
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();

        foreach (var source in sources)
        {
            Destroy(source.gameObject);
        }
    }

    public AudioChannel TryGetChannel(int channelNumber, bool createIfNotExisting = false)
    {
        AudioChannel channel = null;

        if (channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }
        else if (createIfNotExisting)
        {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }
}
