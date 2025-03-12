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

    public void UpdateAudioMusic()
    {
        if (rsoContentSave.Value.audioMusic == 0)
        {
            rsoContentSave.Value.audioMusic = 10;
        }
        else if (rsoContentSave.Value.audioMusic == 10)
        {
            rsoContentSave.Value.audioMusic = 20;
        }
        else if (rsoContentSave.Value.audioMusic == 20)
        {
            rsoContentSave.Value.audioMusic = 30;
        }
        else if (rsoContentSave.Value.audioMusic == 30)
        {
            rsoContentSave.Value.audioMusic = 40;
        }
        else if (rsoContentSave.Value.audioMusic == 40)
        {
            rsoContentSave.Value.audioMusic = 50;
        }
        else if (rsoContentSave.Value.audioMusic == 50)
        {
            rsoContentSave.Value.audioMusic = 60;
        }
        else if (rsoContentSave.Value.audioMusic == 60)
        {
            rsoContentSave.Value.audioMusic = 70;
        }
        else if (rsoContentSave.Value.audioMusic == 70)
        {
            rsoContentSave.Value.audioMusic = 80;
        }
        else if (rsoContentSave.Value.audioMusic == 80)
        {
            rsoContentSave.Value.audioMusic = 90;
        }
        else if (rsoContentSave.Value.audioMusic == 90)
        {
            rsoContentSave.Value.audioMusic = 100;
        }
        else if (rsoContentSave.Value.audioMusic == 100)
        {
            rsoContentSave.Value.audioMusic = 0;
        }

        audioMixer.SetFloat("Music", 40 * Mathf.Log10(Mathf.Max(rsoContentSave.Value.audioMusic, 1) / 100));
        textAudioMusic.text = rsoContentSave.Value.audioMusic.ToString() + "%";

        rseSaveData.Call();
    }

    public void UpdateAudioSounds()
    {
        if (rsoContentSave.Value.audioSounds == 0)
        {
            rsoContentSave.Value.audioSounds = 10;
        }
        else if (rsoContentSave.Value.audioSounds == 10)
        {
            rsoContentSave.Value.audioSounds = 20;
        }
        else if (rsoContentSave.Value.audioSounds == 20)
        {
            rsoContentSave.Value.audioSounds = 30;
        }
        else if (rsoContentSave.Value.audioSounds == 30)
        {
            rsoContentSave.Value.audioSounds = 40;
        }
        else if (rsoContentSave.Value.audioSounds == 40)
        {
            rsoContentSave.Value.audioSounds = 50;
        }
        else if (rsoContentSave.Value.audioSounds == 50)
        {
            rsoContentSave.Value.audioSounds = 60;
        }
        else if (rsoContentSave.Value.audioSounds == 60)
        {
            rsoContentSave.Value.audioSounds = 70;
        }
        else if (rsoContentSave.Value.audioSounds == 70)
        {
            rsoContentSave.Value.audioSounds = 80;
        }
        else if (rsoContentSave.Value.audioSounds == 80)
        {
            rsoContentSave.Value.audioSounds = 90;
        }
        else if (rsoContentSave.Value.audioSounds == 90)
        {
            rsoContentSave.Value.audioSounds = 100;
        }
        else if (rsoContentSave.Value.audioSounds == 100)
        {
            rsoContentSave.Value.audioSounds = 0;
        }

        audioMixer.SetFloat("Sound", 40 * Mathf.Log10(Mathf.Max(rsoContentSave.Value.audioSounds, 1) / 100));
        textAudioSounds.text = rsoContentSave.Value.audioSounds.ToString() + "%";

        rseSaveData.Call();
    }
}