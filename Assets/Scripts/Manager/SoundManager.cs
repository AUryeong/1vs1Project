using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{
    SE,
    BGM,
    END
}

public class SoundManager : Singleton<SoundManager>
{
    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    Dictionary<SoundType, float> Volumes = new Dictionary<SoundType, float>()
    {
        { SoundType.SE, 1 },
        { SoundType.BGM, 1 }
    };

    Dictionary<SoundType, AudioSource> AudioSources = new Dictionary<SoundType, AudioSource>();

    public override void OnReset()
    {
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            GameObject Se = new GameObject("SE");
            Se.transform.parent = transform;
            Se.AddComponent<AudioSource>();
            AudioSources[SoundType.SE] = Se.GetComponent<AudioSource>();
            Volumes[SoundType.SE] = SaveManager.Instance.saveData.sfxVolume;
            VolumeChange(SoundType.SE, SaveManager.Instance.saveData.sfxVolume);

            GameObject Bgm = new GameObject("BGM");
            Bgm.transform.parent = transform;
            Bgm.AddComponent<AudioSource>().loop = true;
            AudioSources[SoundType.BGM] = Bgm.GetComponent<AudioSource>();
            VolumeChange(SoundType.BGM, SaveManager.Instance.saveData.bgmVolume);

            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/");
            foreach (AudioClip clip in clips)
                sounds[clip.name] = clip;
        }
    }

    public void VolumeChange(SoundType soundType, float volume)
    {
        Volumes[soundType] = volume;
        AudioSources[soundType].volume = Volumes[soundType];
        switch (soundType)
        {
            case SoundType.SE:
                SaveManager.Instance.saveData.sfxVolume = volume;
                break;
            case SoundType.BGM:
                SaveManager.Instance.saveData.bgmVolume = volume;
                break;
            case SoundType.END:
            default:
                break;
        }
    }

    public void PlaySound(string clipName, SoundType ClipType = SoundType.SE, float Volume = 1, float Pitch = 1)
    {
        if (ClipType != SoundType.SE && ClipType != SoundType.END)
        {
            if (clipName == "")
            {
                AudioSources[ClipType].Stop();
                return;
            }

            AudioSources[ClipType].clip = sounds[clipName];
            AudioSources[ClipType].volume = Volumes[ClipType] * Volume;
            AudioSources[ClipType].Play();
        }
        else
        {
            if (clipName == "")
            {
                AudioSources[ClipType].Stop();
                return;
            }

            AudioSources[ClipType].pitch = Pitch;
            AudioSources[ClipType].PlayOneShot(sounds[clipName], Volume);
        }
    }
}