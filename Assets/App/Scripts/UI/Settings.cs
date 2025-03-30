using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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
    [SerializeField] private Slider sliderAudioMusic;
    [SerializeField] private Slider sliderAudioSounds;

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
        sliderAudioMusic.value = rsoSettingsSaved.Value.audioMusic / 5;
        textAudioSounds.text = rsoSettingsSaved.Value.audioSounds.ToString() + "%";
        sliderAudioSounds.value = rsoSettingsSaved.Value.audioSounds / 5;
    }

    public void UpdateScreenShake()
    {
        if (rsoSettingsSaved.Value.screenShake)
        {
            textScreenShake.text = "Off";
        }
        else
        {
            textScreenShake.text = "On";
        }

        rsoSettingsSaved.Value.screenShake = !rsoSettingsSaved.Value.screenShake;

        rseSaveData.Call(saveSettingsName, true);
    }

    public void UpdateFullScreen()
    {
        if (rsoSettingsSaved.Value.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            textFullScreen.text = "Off";
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            textFullScreen.text = "On";
        }

        rsoSettingsSaved.Value.fullScreen = !rsoSettingsSaved.Value.fullScreen;

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