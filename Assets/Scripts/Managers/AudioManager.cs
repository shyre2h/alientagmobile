using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource bgSource, sfxSource;
    public AudioClip bgMusic;

    private void Awake()
    {
        instance = this;
    }

    public void PlayBackGroundTrack(bool value, float volume = 1f)
    {
        if (value)
        {
            bgSource.clip = bgMusic;
            bgSource.volume = volume;
            if (!bgSource.isPlaying)
                bgSource.Play();
        }
        else
            bgSource.Stop();
    }


    public void PlaySFX(AudioClip clip, bool stopPrevious = false, float volume = 0.5f)
    {
        if (stopPrevious)
            sfxSource.Stop();
        sfxSource.PlayOneShot(clip, volume);
    }
}
