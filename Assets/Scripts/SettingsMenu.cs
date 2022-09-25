using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public void setVolume(float volume){
        audioMixer.SetFloat("Volume",volume);
    }
    public void setFullScreen(bool isFullScreen){
        Screen.fullScreen = isFullScreen;
        Screen.SetResolution(1920,1080,isFullScreen);
    }
}
