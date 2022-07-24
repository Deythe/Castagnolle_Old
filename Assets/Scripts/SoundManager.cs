using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private AudioSource ambianceTheme;
    [SerializeField] private AudioSource voiceLine;
    [SerializeField] private AudioSource voiceLine2;
    
    [SerializeField] private AudioSource[] sfxAudioSources;
    [SerializeField] private AudioClip[] sfxClickClips;
    
    [SerializeField] private bool musicEnabled = true;
    [SerializeField] private bool sfxEnabled = true;

    public bool p_musicEnabled
    {
        get => musicEnabled;
    }
    
    public bool p_sfxEnabled
    {
        get => sfxEnabled;
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }

        musicEnabled = true;
        sfxEnabled = true;
        StarMenuMusic();
    }

    private void Start()
    {
        musicEnabled = true;
        sfxEnabled = true;
        StarMenuMusic();
    }

    public void PlaySFXSound(int index, float volume)
    {
        for (int i = 0; i < sfxAudioSources.Length; i++)
        {
            if (!sfxAudioSources[i].isPlaying)
            {
                sfxAudioSources[i].volume = volume;
                sfxAudioSources[i].clip = sfxClickClips[index];
                sfxAudioSources[i].Play();
                return;
            }
        }
    }

    public void PlayVoiceLine(AudioClip clip)
    {
        if (!voiceLine.isPlaying)
        {
            voiceLine.clip = clip;
            voiceLine.Play();
        }
        else
        {
            if (!voiceLine2.isPlaying)
            {
                voiceLine2.clip = clip;
                voiceLine2.Play();
            }
        }
    }

    public void StopMusic()
    {
        mainMusic.Stop();
        ambianceTheme.Stop();
    }

    public void EnableDisableMusic()
    {
        musicEnabled = !musicEnabled;
        
        mainMusic.mute = !musicEnabled;
        ambianceTheme.mute = !musicEnabled;
    }

    public void StarMenuMusic()
    {
        mainMusic.Stop();
        ambianceTheme.Stop();
        mainMusic.volume = 0.08f;
        ambianceTheme.volume = 0.56f;
        mainMusic.Play();
        ambianceTheme.Play();
    }
    
    public void StarGameMusic()
    {
        mainMusic.Stop();
        ambianceTheme.Stop();
        mainMusic.volume = 0.03f;
        ambianceTheme.volume = 0.24f;
        mainMusic.Play();
        ambianceTheme.Play();
    }
    
    public void EnableDisableSFX()
    {
        sfxEnabled = !sfxEnabled;

        for (int i = 0; i < sfxAudioSources.Length; i++)
        {
            sfxAudioSources[i].mute = !sfxEnabled;
        }
    }
}
