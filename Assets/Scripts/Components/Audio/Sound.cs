using UnityEngine.Audio;
using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    #region Variables
    // The audio file this script will play
    [SerializeField] private AudioClip clip;

    [SerializeField] private string audioName;

    // Audio controls
    [SerializeField] private bool isAudioMuted;
    [Range(0f, 100f)][SerializeField] private float volume;
    [Range(.1f, 4f)][SerializeField] private float pitch;

    // 0 = 2D, 1 = 3D
    [Range(0, 1)][SerializeField] private int spacialBlend;

    [SerializeField] private bool isLooping;

    [SerializeField] private AudioMixerGroup mixerGroup;

    private AudioSource source;

    #endregion Variables

    #region GetSet
    public AudioClip GetAudioClip()
    { 
        return clip;
    }

    public void SetAudioClip(AudioClip newClip)
    {
        clip = newClip;
    }

    public string GetAudioName()
    {
        return audioName;
    }

    public void SetAudioName(string newName)
    {
        audioName = newName;
    }

    public bool GetIsAudioMuted()
    {
        return isAudioMuted;
    }

    public void SetIsAudioMuted(bool newState)
    {
        isAudioMuted = newState;
    }

    public float GetVolume()
    {
        return volume;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
    }

    public float GetPitch()
    {
        return pitch;
    }

    public void SetPitch(float newPitch)
    {
        pitch = newPitch;
    }

    public int GetSpacialBlend()
    {
        return spacialBlend;
    }

    public void SetSpacialBlend(int newBlend)
    {
        spacialBlend = newBlend;
    }

    public bool GetIsLooping()
    {
        return isLooping;
    }

    public void SetIsLooping(bool newState)
    {
        isLooping = newState;
    }

    public AudioMixerGroup GetAudioMixerGroup()
    {
        return mixerGroup;
    }

    public void SetAudioMixerGroup(AudioMixerGroup newGroup)
    {
        mixerGroup = newGroup;
    }

    public AudioSource GetAudioSource()
    {
        return source;
    }

    public void SetAudioSource(AudioSource newSource)
    {
        source = newSource;
    }

    #endregion GetSet
}