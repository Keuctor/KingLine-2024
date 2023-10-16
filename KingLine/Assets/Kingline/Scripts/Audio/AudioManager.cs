using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundType
{
    BREAKING_1,
    BREAKING_2,
    LEVEL_UP,
}


[Serializable]
public class SoundData
{
    public SoundType Sound;
    public AudioClip Clip;
}

public class AudioManager : Singleton<AudioManager>
{

    public float Volume = 1;
    public List<SoundData> Sounds = new List<SoundData>();
        
    public AudioClip GetClip(SoundType type)
    {
        for(int i=0;i<Sounds.Count;i++)
            if (Sounds[i].Sound == type)
                return Sounds[i].Clip;
        return null;
    }

    public void PlayOnce(SoundType soundType,bool randomPitch,float volume)
    {
        var clip = GetClip(soundType);
        var gm = new GameObject();
        DontDestroyOnLoad(gm);
        var sc = gm.AddComponent<AudioSource>();
        sc.clip = clip;
        sc.volume = Volume*volume;
        sc.pitch = UnityEngine.Random.Range(sc.pitch-0.3f, sc.pitch+0.3f);
        sc.Play();
        Destroy(gm,clip.length);
    }
}
