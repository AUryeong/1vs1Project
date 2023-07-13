using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Se,
    BGM
}

public class AudioInfo
{
    public AudioSource AudioSource { get; private set; }
    public float volume;

    public AudioInfo(AudioSource audioSource)
    {
        this.AudioSource = audioSource;
    }
}

public class SoundManager : Singleton<SoundManager>
{
    protected override bool IsDontDestroying => true;
    private readonly Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    private readonly Dictionary<SoundType, AudioInfo> audioInfos = new Dictionary<SoundType, AudioInfo>();

    protected override void OnCreated()
    {
        base.OnCreated();
        AddAudioType(SoundType.Se, SaveManager.Instance.SaveData.sfxVolume);
        AddAudioType(SoundType.BGM, SaveManager.Instance.SaveData.bgmVolume).loop = true;

        var clips = Resources.LoadAll<AudioClip>("Sounds/");
        foreach (var clip in clips)
            sounds[clip.name] = clip;
    }

    private AudioSource AddAudioType(SoundType soundType, float volume)
    {
        var audioObj = new GameObject(soundType.ToString());
        audioObj.transform.parent = transform;

        audioInfos[soundType] = new AudioInfo(audioObj.AddComponent<AudioSource>())
        {
            volume = volume
        };
        return audioInfos[soundType].AudioSource;
    }

    public void VolumeChange(SoundType soundType, float volume)
    {
        audioInfos[soundType].volume = volume;
        audioInfos[soundType].AudioSource.volume = volume;
        switch (soundType)
        {
            case SoundType.Se:
                SaveManager.Instance.SaveData.sfxVolume = volume;
                break;
            case SoundType.BGM:
                SaveManager.Instance.SaveData.bgmVolume = volume;
                break;
        }
    }

    public void PlaySound(string clipName, SoundType soundType = SoundType.Se, float volume = 1, float pitch = 1)
    {
        if (clipName == "")
        {
            audioInfos[soundType].AudioSource.Stop();
            return;
        }

        audioInfos[soundType].AudioSource.pitch = pitch;
        if (soundType == SoundType.BGM)
        {
            audioInfos[soundType].AudioSource.clip = sounds[clipName];
            audioInfos[soundType].AudioSource.volume = audioInfos[soundType].volume * volume;
            audioInfos[soundType].AudioSource.Play();
            return;
        }

        audioInfos[soundType].AudioSource.PlayOneShot(sounds[clipName], volume);
    }
}