using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static readonly int FIVE_MINUTES = 300;

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

        [SerializeField]
        private bool isLooping;

        [SerializeField]
        private bool playImmediately;

        public AudioClip Clip => audioClip;
        public float StartTime => startTime == -1 ? 0 : startTime / playbackSpeed;
        public float EndTime => endTime == -1 ? Clip.length / playbackSpeed : endTime / playbackSpeed;
        public float Volume => volume;
        public float Duration => EndTime - StartTime;
        public bool HasFade => hasFade;

        public bool IsLooping => isLooping;

        public bool PlayImmediately => playImmediately;

        public float PlaybackSpeed => playbackSpeed;
    }

    [SerializeField]
    private AudioPlayerGroup audioPlayerGroup;

    [SerializeField]
    private Audio[] audioCollection;

    private readonly Dictionary<string, Audio> audioDictionary = new Dictionary<string, Audio>();

    private void Awake()
    {
        if (audioPlayerGroup == null)
        {
            audioPlayerGroup = GameObject.FindGameObjectWithTag("AudioSourceGroup").GetComponent<AudioPlayerGroup>();
        }

        foreach (Audio audio in audioCollection)
        {
            audioDictionary[audio.name] = audio;
        }
    }

    private void Start()
    {
        foreach (Audio audio in audioCollection)
        {
            if (audio.PlayImmediately)
            {
                PlayTrack(audio);
            }
        }
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

    public void StopTrack(string name)
    {
        audioPlayerGroup.Stop(audioDictionary[name]);
    }

    public void StopTrackWithFade(string name)
    {
        audioPlayerGroup.StopTrackWithFade(audioDictionary[name]);
    }
}
