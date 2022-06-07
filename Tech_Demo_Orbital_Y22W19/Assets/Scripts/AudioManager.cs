using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static readonly int TWENTY_MINUTES = 1200;

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
            }
            return instance;
        }
    }

    [Serializable]
    public class Audio
    {
        public string name;

        [SerializeField]
        private AudioClip audioClip;

        [SerializeField]
        private float startTime;

        [SerializeField]
        private float endTime;

        [SerializeField]
        private float volume = 1;

        [SerializeField]
        private float playbackSpeed = 1;

        [SerializeField]
        private bool hasFade;

        public AudioClip Clip => audioClip;
        public float StartTime => startTime == -1 ? 0 : startTime / playbackSpeed;
        public float EndTime => endTime == -1 ? Clip.length / playbackSpeed : endTime / playbackSpeed;
        public float Volume => volume;
        public float Duration => EndTime - StartTime;
        public bool HasFade => hasFade;
        public float PlaybackSpeed => playbackSpeed;
    }

    [SerializeField]
    private Audio[] audioCollection;

    private readonly Dictionary<string, Audio> audioDictionary = new Dictionary<string, Audio>();

    private AudioPlayerGroup audioPlayerGroup;

    private void Awake()
    {
        audioPlayerGroup = GameObject.FindGameObjectWithTag("AudioSourceGroup").GetComponent<AudioPlayerGroup>();

        foreach (Audio audio in audioCollection)
        {
            audioDictionary[audio.name] = audio;
        }
    }

    private void Start()
    {
        PlayTrack("HowlingWind", looping: true);
        PlayTrack("HowlingWind", delay: TWENTY_MINUTES, looping: true);
    }

    public void PlayTrack(string name)
    {
        PlayTrack(audioDictionary[name], 0, false);
    }

    /// <summary>
    /// Plays a track in the scene
    /// </summary>
    /// <param name="name"></param>
    /// <param name="delay">Delay to start the track in seconds</param>
    /// <param name="looping">Whether the track loops after ending</param>
    public void PlayTrack(string name, float delay = 0, bool looping = false)
    {
        PlayTrack(audioDictionary[name], delay, looping);
    }

    /// <summary>
    /// Plays a track in the scene through the audio player group.
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="delay"></param>
    /// <param name="looping"></param>
    private void PlayTrack(Audio audio, float delay = 0, bool looping = false)
    {
        audioPlayerGroup.Play(audio, delay, looping);
    }
}
