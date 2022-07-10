using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

class AudioPlayerGroup : MonoBehaviour
{
    private static readonly float ONE_THIRD = 0.333f;

    private static AudioPlayerGroup instance;

    public static AudioPlayerGroup Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioPlayerGroup>();
            }
            return instance;
        }
    }

    public class AudioPlayer
    {
        private readonly AudioSource audioSource;
        public AudioSource AudioSource => audioSource;

        private Task currentPlayingTask;

        public bool ClipHasEnded => currentPlayingTask != null ? !currentPlayingTask.Running : true;

        public delegate void CompletedAction();

        public event CompletedAction Finished;

        private Task fade;

        public string Name
        {
            get;
            private set;
        }

        public AudioPlayer(AudioSource audioSource)
        {
            this.audioSource = audioSource;
        }


        /// <summary>
        /// Plays the given audio recieved from the AudioManager in this contained AudioSource.
        /// The audio source will be freed up when the clip duration has elapsed.
        /// </summary>
        /// <param name="audio"></param>
        public void Play(AudioManager.Audio audio, bool looping = false)
        {
            Name = audio.name;
            audioSource.clip = audio.Clip;
            audioSource.volume = audio.Volume;
            audioSource.pitch = audio.PlaybackSpeed;

            audioSource.time = audio.StartTime;

            audioSource.loop = looping;
            if (audio.IsLooping)
            {
                audioSource.loop = true;
            }

            if (audio.HasFade)
            {
                fade = new Task(FadeAudio(0, audio.Duration * (1 - ONE_THIRD), audio.Duration * ONE_THIRD));
            }

            currentPlayingTask = new Task(TrackClipProgress(audio.Duration));
            
            audioSource.Play();
            audioSource.SetScheduledEndTime(AudioSettings.dspTime + audio.Duration);
        }

        public void Stop()
        {
            audioSource?.Stop();
            OnFinished();
        }

        private IEnumerator FadeAudio(float targetVolume, float startTime, float duration)
        {
            startTime += Time.time;

            while (Time.time < startTime)
            {
                yield return null;
            }

            float endTime = startTime + duration;
            float startingVolume = audioSource.volume;

            while (Time.time < endTime)
            {
                float fraction = (Time.time - startTime) / duration;    
                audioSource.volume = Mathf.Lerp(startingVolume, targetVolume, fraction);
                yield return null;
            }
        }

        private IEnumerator TrackClipProgress(float duration)
        {
            yield return new WaitForSeconds(duration);
            fade?.Stop();
            OnFinished();
        }

        protected virtual void OnFinished()
        {
            Finished?.Invoke();
        }
    }


    private HashSet<AudioPlayer> audioSourcesPlaying = new HashSet<AudioPlayer>();

    private HashSet<AudioPlayer> audioSourcesDormant = new HashSet<AudioPlayer>();

    private Dictionary<string, AudioPlayer> audioPlaying = new Dictionary<string, AudioPlayer>();

    /// <summary>
    /// Retrieves any available audio player from the audio group.
    /// If there is none, a new audio player will be created.
    /// </summary>
    /// <returns></returns>
    private AudioPlayer GetAvailableAudioPlayer()
    {
        if (audioSourcesDormant.Count < 1)
        {
            audioSourcesDormant.Add(new AudioPlayer(gameObject.AddComponent<AudioSource>()));
        }

        return audioSourcesDormant.First(); // take any audioSource
    }

    public void ChangeToPlaying(AudioPlayer audioPlayer)
    {
        audioSourcesDormant.Remove(audioPlayer);
        audioSourcesPlaying.Add(audioPlayer);
        audioPlaying[audioPlayer.Name] = audioPlayer;
    }


    public void ChangeToDormant(AudioPlayer audioPlayer)
    {
        audioSourcesPlaying.Remove(audioPlayer);
        audioPlaying.Remove(audioPlayer.Name);
        audioSourcesDormant.Add(audioPlayer);
    }

    public void Play(AudioManager.Audio audio, float delay = 0, bool looping = false)
    {
        if (delay > 0)
        {
            StartCoroutine(InvokeLater(audio, delay, looping));
            return;
        }

        AudioPlayer audioPlayer = GetAvailableAudioPlayer();
        audioPlayer.Play(audio, looping);
        ChangeToPlaying(audioPlayer);
        audioPlayer.Finished += () => ChangeToDormant(audioPlayer);
    }

    public void Stop(AudioManager.Audio audio)
    {
        audioPlaying[audio.name].Stop();
    }

    private IEnumerator InvokeLater(AudioManager.Audio audio, float delay, bool looping)
    {
        yield return new WaitForSeconds(delay);
        Play(audio, 0, looping);
    }
}