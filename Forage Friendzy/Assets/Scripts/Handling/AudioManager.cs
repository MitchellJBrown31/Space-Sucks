﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    [SerializeField] private float masterVolume = 1.0f; 
    [SerializeField] private float musicVolume = 1.0f; 
    [SerializeField] private float sfxVolume = 1.0f;
    [SerializeField] private float voiceVolume = 1.0f;
    private Dictionary<AudioCatagories, PoolTypes> catagoryMap = new Dictionary<AudioCatagories, PoolTypes>();

    public event Action event_VolumeValueChanged;

    private void Awake()
    {

        instance = this;

        //catagoryMap.Add(AudioCatagories.Music, PoolTypes.MusicAudioSource);
        catagoryMap.Add(AudioCatagories.SFX, PoolTypes.SFXAudioSource);
    }

    #region Helpers

    public PooledAudioSource LoanOneShotSource(AudioCatagories catagory, AudioClip toPlay)
    {
            
        PooledAudioSource loanedSource = ObjectPoolManager.Instance.GetPooledObjectComponent<PooledAudioSource>(GetMappedPoolType(catagory));
        loanedSource.gameObject.SetActive(true);
        if (toPlay != null)
            loanedSource.PlayAudio(toPlay);
        return loanedSource;
    }

    public PooledAudioSource LoanLoopingSource(AudioCatagories catagory, AudioClip toPlay)
    {

        PooledAudioSource loanedSource = ObjectPoolManager.Instance.GetPooledObjectComponent<PooledAudioSource>(GetMappedPoolType(catagory));
        loanedSource.gameObject.SetActive(true);
        if(toPlay != null)
            loanedSource.PlayAudioLoop(toPlay);
        return loanedSource;
    }

    public float GetVolume(AudioCatagories catagory)
    {
        switch(catagory)
        {
            case AudioCatagories.Master:
                return GetMasterVolume();
            case AudioCatagories.Music:
                return GetMusicVolume();
            case AudioCatagories.SFX:
                return GetSFXVolume();
            case AudioCatagories.Voice:
                return GetVoiceVolume();
        }

        return 1.0f;
    }

    private PoolTypes GetMappedPoolType(AudioCatagories key)
    {
        return catagoryMap[key];
    }

    #endregion

    #region Setters

    public void SetMaster(float newValue)
    {
        masterVolume = newValue;
        event_VolumeValueChanged?.Invoke();
    }

    public void SetMusic(float newValue)
    {
        musicVolume = newValue;
        event_VolumeValueChanged?.Invoke();
    }

    public void SetSFX(float newValue)
    {
        sfxVolume = newValue;
        event_VolumeValueChanged?.Invoke();
    }

    public void SetVoice(float newValue)
    {
        voiceVolume = newValue;
        event_VolumeValueChanged?.Invoke();
    }

    #endregion

    #region Getters

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public float GetVoiceVolume()
    {
        return voiceVolume;
    }

    #endregion

}

public enum AudioCatagories
{
    Master,
    Music,
    SFX,
    Voice
}