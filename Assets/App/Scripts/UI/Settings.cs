using BT.Save;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textScreenShake;
    [SerializeField] private TextMeshProUGUI textFullScreen;
    [SerializeField] private TextMeshProUGUI textAudioMusic;
    [SerializeField] private TextMeshProUGUI textAudioSounds;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Output")]
    [SerializeField] private RSO_ContentSaved rsoContentSave;
    [SerializeField] private RSE_SaveData rseSaveData;

    private void Start()
    {
        UpdateUISettings();
    }

    private void UpdateUISettings()
    {
        if (rsoContentSave.Value.screenShake)
        {
            textScreenShake.text = "On";
        }
        else if (!rsoContentSave.Value.screenShake)
        {
            textScreenShake.text = "Off";
        }

        if (rsoContentSave.Value.fullScreen)
        {
            textFullScreen.text = "On";
        }
        else if (!rsoContentSave.Value.fullScreen)
        {
            textFullScreen.text = "Off";
        }

        textAudioMusic.text = rsoContentSave.Value.audioMusic.ToString();
        textAudioSounds.text = rsoContentSave.Value.audioSounds.ToString();
    }

    public void UpdateScreenShake()
    {
        if (rsoContentSave.Value.screenShake)
        {
            rsoContentSave.Value.screenShake = false;
            textScreenShake.text = "Off";
        }
        else
        {
            rsoContentSave.Value.screenShake = true;
            textScreenShake.text = "On";
        }

        rseSaveData.Call();
    }

    public void UpdateFullScreen()
    {
        if (rsoContentSave.Value.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            rsoContentSave.Value.fullScreen = false;
            textFullScreen.text = "Off";
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            rsoContentSave.Value.fullScreen = true;
            textFullScreen.text = "On";
        }

        Screen.fullScreen = rsoContentSave.Value.fullScreen;
        rseSaveData.Call();
    }

    public void UpdateAudioMusic()
    {
        if (rsoContentSave.Value.audioMusic == 0)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(10, 1) / 100));
            textAudioMusic.text = "10";
        }
        else if (rsoContentSave.Value.audioMusic == 10)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(20, 1) / 100));
            textAudioMusic.text = "20";
        }
        else if (rsoContentSave.Value.audioMusic == 20)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(30, 1) / 100));
            textAudioMusic.text = "30";
        }
        else if (rsoContentSave.Value.audioMusic == 30)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(40, 1) / 100));
            textAudioMusic.text = "40";
        }
        else if (rsoContentSave.Value.audioMusic == 40)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(50, 1) / 100));
            textAudioMusic.text = "50";
        }
        else if (rsoContentSave.Value.audioMusic == 50)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(60, 1) / 100));
            textAudioMusic.text = "60";
        }
        else if (rsoContentSave.Value.audioMusic == 60)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(70, 1) / 100));
            textAudioMusic.text = "70";
        }
        else if (rsoContentSave.Value.audioMusic == 70)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(80, 1) / 100));
            textAudioMusic.text = "80";
        }
        else if (rsoContentSave.Value.audioMusic == 80)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(90, 1) / 100));
            textAudioMusic.text = "90";
        }
        else if (rsoContentSave.Value.audioMusic == 90)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(100, 1) / 100));
            textAudioMusic.text = "100";
        }
        else if (rsoContentSave.Value.audioMusic == 100)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(0, 1) / 100));
            textAudioMusic.text = "0";
        }

        rseSaveData.Call();
    }

    public void UpdateAudioSounds()
    {
        if (rsoContentSave.Value.audioMusic == 0)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(10, 1) / 100));
            textAudioMusic.text = "10";
        }
        else if (rsoContentSave.Value.audioMusic == 10)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(20, 1) / 100));
            textAudioMusic.text = "20";
        }
        else if (rsoContentSave.Value.audioMusic == 20)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(30, 1) / 100));
            textAudioMusic.text = "30";
        }
        else if (rsoContentSave.Value.audioMusic == 30)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(40, 1) / 100));
            textAudioMusic.text = "40";
        }
        else if (rsoContentSave.Value.audioMusic == 40)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(50, 1) / 100));
            textAudioMusic.text = "50";
        }
        else if (rsoContentSave.Value.audioMusic == 50)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(60, 1) / 100));
            textAudioMusic.text = "60";
        }
        else if (rsoContentSave.Value.audioMusic == 60)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(70, 1) / 100));
            textAudioMusic.text = "70";
        }
        else if (rsoContentSave.Value.audioMusic == 70)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(80, 1) / 100));
            textAudioMusic.text = "80";
        }
        else if (rsoContentSave.Value.audioMusic == 80)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(90, 1) / 100));
            textAudioMusic.text = "90";
        }
        else if (rsoContentSave.Value.audioMusic == 90)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(100, 1) / 100));
            textAudioMusic.text = "100";
        }
        else if (rsoContentSave.Value.audioMusic == 100)
        {
            audioMixer.SetFloat("Volume", 40 * Mathf.Log10(Mathf.Max(0, 1) / 100));
            textAudioMusic.text = "0";
        }

        rseSaveData.Call();
    }
}