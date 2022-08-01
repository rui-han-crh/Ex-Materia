using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustMasterVolume : MonoBehaviour
{
    [Range(0f, 1f), SerializeField]
    private float masterVolume = 1;

    private void Start()
    {
        SetVolume();
    }

    public void SetVolume(float volume)
    {
        masterVolume = volume;
        SetVolume();
    }

    public void SetVolume()
    {
        AudioListener.volume = masterVolume;
    }

}
