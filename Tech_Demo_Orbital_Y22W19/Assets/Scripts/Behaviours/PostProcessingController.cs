using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour, ISaveable
{
    [SerializeField]
    private GameObject globalVolumePrefab;

    [SerializeField]
    private bool runtimeInstantiateVolume = true;

    [SerializeField]
    private GameObject globalVolume;

    private Volume volume;
    private ColorAdjustments colorAdjustments;
    private Tonemapping tonemapping;

    public void Reset()
    {
        GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
    }

    public void LoadData()
    {
        if (SaveFile.file.HasData(typeof(PostProcessingController), "brightness")) 
        {
            colorAdjustments.postExposure.value = SaveFile.file.Load<float>(typeof(PostProcessingController), "brightness");
        }

        if (SaveFile.file.HasData(typeof(PostProcessingController), "tonemapping"))
        {
            tonemapping.active = SaveFile.file.Load<bool>(typeof(PostProcessingController), "tonemapping");
        }
    }

    public void SaveData()
    {
        SaveFile.file.Save(typeof(PostProcessingController), "brightness", colorAdjustments.postExposure.value);
        SaveFile.file.Save(typeof(PostProcessingController), "tonemapping", tonemapping.active);
    }

    private void Awake()
    {
        if (runtimeInstantiateVolume)
        {
            globalVolume = Instantiate(globalVolumePrefab, transform);
        }

        volume = globalVolume.GetComponent<Volume>();

        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out tonemapping);
    }
}
