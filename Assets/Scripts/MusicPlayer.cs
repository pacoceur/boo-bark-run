using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    private int lastPlayedIndex = -1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomAudio();
        bool isMuted = PlayerPrefs.GetInt("isMuted", 0) != 0;
        audioSource.mute = isMuted;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomAudio();
        }
    }

    void PlayRandomAudio()
    {
        if (audioClips.Length == 1)
        {
            audioSource.clip = audioClips[0];
        }
        else
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, audioClips.Length);
            } while (randomIndex == lastPlayedIndex);

            lastPlayedIndex = randomIndex;

            audioSource.clip = audioClips[randomIndex];
        }

        audioSource.Play();
    }

    public void Mute()
    {
        bool isMuted = PlayerPrefs.GetInt("isMuted", 0) != 0;
        audioSource.mute = !isMuted;
        PlayerPrefs.SetInt("isMuted", (isMuted ? 0 : 1));
    }
}
