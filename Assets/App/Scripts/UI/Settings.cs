using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

        textAudioMusic.text = rsoContentSave.Value.audioMusic.ToString() + "%";
        textAudioSounds.text = rsoContentSave.Value.audioSounds.ToString() + "%";
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

    public void UpdateAudioMusic(float val)
    {
        rsoContentSave.Value.audioMusic = val * 5;

        audioMixer.SetFloat("Music", 40 * Mathf.Log10(Mathf.Max(rsoContentSave.Value.audioMusic, 1) / 100));
        textAudioMusic.text = rsoContentSave.Value.audioMusic.ToString() + "%";

        rseSaveData.Call();
    }

    public void UpdateAudioSounds(float val)
    {
        rsoContentSave.Value.audioSounds = val * 5;

        audioMixer.SetFloat("Sound", 40 * Mathf.Log10(Mathf.Max(rsoContentSave.Value.audioSounds, 1) / 100));
        textAudioSounds.text = rsoContentSave.Value.audioSounds.ToString() + "%";

        rseSaveData.Call();
    }
}