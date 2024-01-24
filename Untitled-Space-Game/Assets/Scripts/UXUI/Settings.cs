using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Video")]
    List<Resolution> resolutions = new();
    [SerializeField] TMP_Dropdown resDropDown;
    [SerializeField] int currentRes;
    int lastRes;

    [SerializeField] TMP_Dropdown fullScreenDrowdown;
    [SerializeField] int currentFullscreen;
    int lastFullscreen;

    [SerializeField] Slider fpsSlider;


    // Start is called before the first frame update
    void Start()
    {
        GetAndSetResolution();
        // StartSettingsChange();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Audio
    public void SetMasterVol(float masterLvl)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(masterLvl) * 20);
        PlayerPrefs.SetFloat("MasterVol", masterLvl);
    }

    public void SetMusicVol(float musicLvl)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(musicLvl) * 20);
        PlayerPrefs.SetFloat("MusicVol", musicLvl);
    }
    public void SetSFXVol(float sfxLvl)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(sfxLvl) * 20);
        PlayerPrefs.SetFloat("SfxVol", sfxLvl);
    }
    #endregion

    #region Video
    #region Resolution 
    void GetAndSetResolution()
    {
        Resolution[] tempRes = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();

        for (int i = tempRes.Length - 1; i > 0; i--)
        {
            resolutions.Add(tempRes[i]);
        }
        resDropDown.ClearOptions();

        List<string> options = new();

        for (int i = 0; i < resolutions.Count; i++)
        {
            string Option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(Option);
        }
        //options.Reverse();
        resDropDown.AddOptions(options);
        resDropDown.value = currentRes;
        resDropDown.RefreshShownValue();

        Screen.SetResolution(resolutions[currentRes].width, resolutions[currentRes].height, true);
        fpsSlider.maxValue = Screen.resolutions[currentRes].refreshRate;

        Application.targetFrameRate = Screen.resolutions[currentRes].refreshRate;
        fpsSlider.value = fpsSlider.maxValue;

    }

    void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        resDropDown.value = index;
        resDropDown.RefreshShownValue();
        SetScreenOptions(PlayerPrefs.GetInt("FullscreenIndex"));
        currentRes = index;
    }

    void GetResolution()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            SetResolution(PlayerPrefs.GetInt("ResolutionIndex"));
        }
        else
        {
            SetResolution(0);
        }
    }

    public void NewResolution(int index)
    {
        resDropDown.value = index;
        resDropDown.RefreshShownValue();
    }

    void SetScreenOptions(int index)
    {
        switch (index)
        {
            case 0:
                {
                    FullScreen();
                }
                break;
            case 1:
                {
                    Borderless();
                }
                break;
            case 2:
                {
                    Windowed();
                }
                break;
        }
    }

    void FullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }
    void Borderless()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }
    void Windowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
    }
    #endregion

    #endregion



}
