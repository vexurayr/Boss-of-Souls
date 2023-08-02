using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Variables
    public static AudioManager instance { get; private set; }

    // Audio is at times buggy
    [SerializeField] private Sound[] sounds;

    #endregion Variables

    #region MonoBehaviours
    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize each sound and its settings
        foreach (Sound sound in sounds)
        {
            GameObject audioSource = new GameObject();
            audioSource.name = sound.GetAudioName();
            audioSource.transform.parent = transform;

            sound.SetAudioSource(audioSource.AddComponent<AudioSource>());
            sound.GetAudioSource().clip = sound.GetAudioClip();

            sound.GetAudioSource().mute = sound.GetIsAudioMuted();
            sound.GetAudioSource().volume = sound.GetVolume();
            sound.GetAudioSource().pitch = sound.GetPitch();
            sound.GetAudioSource().spatialBlend = sound.GetSpacialBlend();
            sound.GetAudioSource().loop = sound.GetIsLooping();
            sound.GetAudioSource().outputAudioMixerGroup = sound.GetAudioMixerGroup();
        }
    }

    #endregion MonoBehaviours

    #region PlaySounds
    public void PlayButtonSound()
    {
        PlaySound2D("Button");
    }

    public void PlaySound2D(string audioName)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);
        
        try
        {
            sound.SetSpacialBlend(0);
            sound.GetAudioSource().Play(0);
        }
        catch (Exception)
        { }
    }

    public void PlaySound3D(string audioName, Transform soundTransform)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);
        
        try
        {
            if (sound.GetAudioSource().loop)
            {
                PlayLoopingSound(audioName, soundTransform);
            }
            else
            {
                sound.GetAudioSource().transform.position = soundTransform.position;
                sound.GetAudioSource().Play();
            }
        }
        catch (Exception)
        {}
    }

    public void PlayLoopingSound(string audioName, Transform soundTransform)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);

        try
        {
            sound.GetAudioSource().transform.position = soundTransform.position;

            if (!IsSoundAlreadyPlaying(audioName))
            {
                sound.GetAudioSource().Play();
            }
        }
        catch (Exception)
        {}
    }

    #endregion PlaySounds

    #region StopSounds
    public void StopSound(string audioName)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);

        try
        {
            sound.GetAudioSource().Stop();
        }
        catch (Exception)
        {}
    }

    #endregion StopSounds

    #region HelperFunctions
    // To handle looping sounds
    public bool IsSoundAlreadyPlaying(string audioName)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);

        try
        {
            return sound.GetAudioSource().isPlaying;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void StopSoundIfItsPlaying(string audioName)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);

        try
        {
            if (IsSoundAlreadyPlaying(audioName))
            {
                sound.GetAudioSource().Stop();
            }
        }
        catch (Exception)
        {}
    }

    public void RefreshAudioTransform(string audioName, Transform soundTransform)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetAudioName() == audioName);

        try
        {
            sound.GetAudioSource().transform.position = soundTransform.position;
        }
        catch (Exception)
        { }
    }

    #endregion HelperFunctions
}