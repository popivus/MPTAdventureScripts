using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource[] stepSounds, runSounds, sounds, music;
    public AudioMixerGroup mixer;
    private System.Random random;
    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        mixer.audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("Music", 0));
        mixer.audioMixer.SetFloat("Sound", PlayerPrefs.GetFloat("Sound", 0));
        random = new System.Random();
    }

    public void PlayMusic(int number)
    {
        music[number].Play();
    }

    public void PlaySound(int number)
    {
        sounds[number].Play();
    }

    public void CarSound(bool isPlay)
    {
        if (isPlay && !UIController.instance.isPaused) 
        {
            if (!sounds[2].isPlaying) sounds[2].Play();
        }
        else sounds[2].Stop();
    }

    public void PlayStepSound()
    {
        bool isPlaying = false;
        foreach(AudioSource step in stepSounds) if (step.isPlaying) isPlaying = true;
        if (!isPlaying) stepSounds[random.Next(0, stepSounds.Length - 1)].Play();
    }

    public void PlayRunSound()
    {
        bool isPlaying = false;
        foreach(AudioSource run in runSounds) if (run.isPlaying) isPlaying = true;
        if (!isPlaying) runSounds[random.Next(0, runSounds.Length - 1)].Play();
    }

    public void SetMusicLevel(float value)
    {
        mixer.audioMixer.SetFloat("Music", value);
    }

    public void SetSoundLevel(float value)
    {
        mixer.audioMixer.SetFloat("Sound", value);
    }
}
