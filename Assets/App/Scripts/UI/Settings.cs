using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, SaveName] private string saveSettingsName;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI textScreenShake;
    [SerializeField] private TextMeshProUGUI textFullScreen;
    [SerializeField] private TextMeshProUGUI textAudioMusic;
    [SerializeField] private TextMeshProUGUI textAudioSounds;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Output")]
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;
    [SerializeField] private RSE_SaveData rseSaveData;

    private void Start()
    {
        UpdateUISettings();
    }

    private void UpdateUISettings()
    {
        if (rsoSettingsSaved.Value.screenShake)
        {
            textScreenShake.text = "On";
        }
        else if (!rsoSettingsSaved.Value.screenShake)
        {
            textScreenShake.text = "Off";
        }

        if (rsoSettingsSaved.Value.fullScreen)
        {
            textFullScreen.text = "On";
        }
        else if (!rsoSettingsSaved.Value.fullScreen)
        {
            textFullScreen.text = "Off";
        }

        textAudioMusic.text = rsoSettingsSaved.Value.audioMusic.ToString() + "%";
        textAudioSounds.text = rsoSettingsSaved.Value.audioSounds.ToString() + "%";
    }

    public void UpdateScreenShake()
    {
        if (rsoSettingsSaved.Value.screenShake)
        {
            rsoSettingsSaved.Value.screenShake = false;
            textScreenShake.text = "Off";
        }
        else
        {
            rsoSettingsSaved.Value.screenShake = true;
            textScreenShake.text = "On";
        }

        rseSaveData.Call(saveSettingsName, true);
    }

    public void UpdateFullScreen()
    {
        if (rsoSettingsSaved.Value.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            rsoSettingsSaved.Value.fullScreen = false;
            textFullScreen.text = "Off";
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            rsoSettingsSaved.Value.fullScreen = true;
            textFullScreen.text = "On";
        }

        Screen.fullScreen = rsoSettingsSaved.Value.fullScreen;
        rseSaveData.Call(saveSettingsName, true);
    }

    public void UpdateAudioMusic(float val)
    {
        rsoSettingsSaved.Value.audioMusic = val * 5;

        audioMixer.SetFloat("Music", 40 * Mathf.Log10(Mathf.Max(rsoSettingsSaved.Value.audioMusic, 1) / 100));
        textAudioMusic.text = rsoSettingsSaved.Value.audioMusic.ToString() + "%";

        rseSaveData.Call(saveSettingsName, true);
    }

    public void UpdateAudioSounds(float val)
    {
        rsoSettingsSaved.Value.audioSounds = val * 5;

        audioMixer.SetFloat("Sound", 40 * Mathf.Log10(Mathf.Max(rsoSettingsSaved.Value.audioSounds, 1) / 100));
        textAudioSounds.text = rsoSettingsSaved.Value.audioSounds.ToString() + "%";

        rseSaveData.Call(saveSettingsName, true);
    }
}