using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour
{
    [Header("UI")]
    public Toggle fullscreenToggle;

    private void Start()
    {
        
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        ApplyFullscreen(isFullscreen);

        
        fullscreenToggle.isOn = isFullscreen;

        
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
    }

    public void OnFullscreenToggle(bool isFullscreen)
    {
        ApplyFullscreen(isFullscreen);

       
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.fullScreen = false;

            
            Screen.SetResolution(3840, 2160, false);
        }
    }
}