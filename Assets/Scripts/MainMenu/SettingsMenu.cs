using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Localization;


public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private Slider musicSlider, vfxSlider, fadeSlider;
    [SerializeField]
    private Toggle fullScreen;
    float musicVolume, vfxVolume, fade;
    Resolution[] resolutions;

    void Awake()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                  && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetFade(float num)
    {
        fade = num;
    }

    public void SetVfx(float volume)
    {
        vfxVolume = volume;
    }

    public void SetMusic(float volume)
    {
        musicVolume = volume;
        GetComponent<AudioSource>().volume = musicSlider.value;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MusicPreference", musicVolume);
        PlayerPrefs.SetFloat("vfxPreference", vfxVolume);
        PlayerPrefs.SetFloat("Fade", fade);
        Debug.Log(PlayerPrefs.GetFloat("MusicPreference"));
        Debug.Log("Saved!");
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("FullscreenPreference")) {
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            fullScreen.isOn = Screen.fullScreen;
        }
        if (PlayerPrefs.HasKey("MusicPreference")) {
            musicVolume = PlayerPrefs.GetFloat("MusicPreference");
            musicSlider.value = musicVolume;
            GetComponent<AudioSource>().volume = musicVolume;
        }
        else 
            musicVolume = 1;
        if (PlayerPrefs.HasKey("vfxPreference")) {
            vfxVolume = PlayerPrefs.GetFloat("vfxPreference");
            vfxSlider.value = vfxVolume;
        }
        else
            vfxVolume = 1;
        if(PlayerPrefs.HasKey("Fade")) {
            fade = PlayerPrefs.GetFloat("Fade");
            fadeSlider.value = fade;
        }
        else
            fade = 0.5f;
    }
}
