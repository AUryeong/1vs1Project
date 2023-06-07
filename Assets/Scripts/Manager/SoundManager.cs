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
    float Volumes = 1f;
    Dictionary<SoundType, AudioSource> AudioSources = new Dictionary<SoundType, AudioSource>();

    public override void OnReset()
    {
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            Volumes = 0;

            GameObject Se = new GameObject("SE");
            Se.transform.parent = transform;
            Se.AddComponent<AudioSource>();
            AudioSources[SoundType.SE] = Se.GetComponent<AudioSource>();
            GameObject Bgm = new GameObject("BGM");
            Bgm.transform.parent = transform;
            Bgm.AddComponent<AudioSource>().loop = true;
            AudioSources[SoundType.BGM] = Bgm.GetComponent<AudioSource>();

            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/");
            foreach (AudioClip clip in clips)
                sounds[clip.name] = clip;
        }
    }

    public void VolumeChange(float volume)
    {
        Volumes = volume;
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
            AudioSources[ClipType].volume *= Volume;
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
    private void Update()
    {
        AudioSources[SoundType.BGM].volume = Volumes;
        AudioSources[SoundType.SE].volume = Volumes;
    }
}