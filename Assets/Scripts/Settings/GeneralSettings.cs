using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Holds general settings for audio and resolution.
 */
public class GeneralSettings : MonoBehaviour
{
    [Header("ExternalStructure")]
    public AudioSource[] controlledAudioSources;

    public ErrorPanel error;

    [Header("Options")] public Vector2Int[] displayResolutions; //Legal values for screen resolution in order of preference (most to least).


    [Header("Dynamic Values")]
    public Vector2Int currentRes;
    public float currentVolume = 0.5f;
    public List<Vector2Int> supportedResolutions;
    public bool windowed;


    void Awake()
    {
        supportedResolutions = GenerateSupportedResolutions();
        bool fs = Screen.fullScreen;
        Screen.fullScreen = false;
        if (supportedResolutions.Count == 0)
        {
            #if !UNITY_WEBGL
            error.Populate("Your screen does not support any of our recommended resolutions. You may continue to use the application, but there may be issues with button sensitivity.");
            #endif
        }
        else
        {
            OnChangeResolution(supportedResolutions[0], fs);
            Debug.Log($"Set screen resolution to best supported resolution, {supportedResolutions[0].x} x {supportedResolutions[0].y}");
        }

        OnChangeVolume(currentVolume);
    }


    public Vector2 GetResolution()
    {
        return currentRes;
    }

    public float GetSoundVolume()
    {
        return currentVolume;
    }

    public int GetCurrentResolutionIndex()
    {
        return Array.IndexOf(displayResolutions, currentRes);
    }

    public void OnChangeVolume(float val)
    {
        currentVolume = val;

        foreach (AudioSource src in controlledAudioSources)
        {
            src.volume = val;
        }
    }
        
    public void OnChangeResolution(Vector2Int res, bool win)
    {
        currentRes = res;
        windowed = win;
        Screen.SetResolution(currentRes.x, currentRes.y, !windowed);
    }

    private List<Vector2Int> GenerateSupportedResolutions()
    {
        HashSet<Vector2Int> supported = new HashSet<Vector2Int>(Screen.resolutions.Select(res => new Vector2Int(res.width, res.height)).ToList());
        return displayResolutions.Where(res => supported.Contains(res)).ToList();
    }
}
