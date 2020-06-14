using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioGroup
{
    UI,
    Environmental
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f,1f)]
    public float volume;

    public bool playOnAwake;
    public bool loop;

    public AudioGroup audioGroup;

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    
    public static AudioManager Instance;

    
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayClip("Lava");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayClip(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s?.source.Play();
    }
    
    public void ButtonHoverSound()
    {
        PlayClip("ButtonHover");
    }
    
    public void ButtonClickSound()
    {
        PlayClip("ButtonClick");
    }

    public void SilenceAllSounds()
    {
        AudioSource[] playingSounds = GetComponents<AudioSource>();
        foreach (AudioSource source in playingSounds)
        {
            source.DOFade(0, 1f);
        }
    }

    public void DisableEnvironmentalAudio()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.audioGroup == AudioGroup.Environmental)
            {
                sound.source.DOFade(0, 1f);
            }
        }
    }
}
