using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource bgSource, sfxSource;
    public AudioClip bgMusic;
    public float bgVolume { private set; get; }
    public float sfxVolume { private set; get; }

    private void Awake()
    {
        instance = this;
        bgVolume = PlayerPrefs.GetFloat("bg_volume", 0.3f);
        sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 0.2f);
    }

    public void PlayBackGroundTrack(bool value)
    {
        if (value)
        {
            bgSource.clip = bgMusic;
            bgSource.volume = bgVolume;
            if (!bgSource.isPlaying)
                bgSource.Play();
        }
        else
            bgSource.Stop();
    }


    public void PlaySFX(AudioClip clip, bool stopPrevious = false)
    {
        if (stopPrevious)
            sfxSource.Stop();
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetBGVolume(float value)
    {
        bgVolume = value;
        bgSource.volume = value;
        PlayerPrefs.SetFloat("bg_volume", value);

    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("sfx_volume", value);

    }

}
