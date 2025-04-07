using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int startingAudioObjectsCount;

    [SerializeField] float transitionFadeOutDelay;
    [SerializeField] float transitionFadeInDelay;

    [Header("References")]
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundMixerGroup;

    [Header("Input")]
    [SerializeField] RSE_PlayClipAt rsePlayClipAt;
    [SerializeField] RSE_OnFightStart rseOnFightStart;
    [SerializeField] RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;
    [SerializeField] RSE_ChangeAmbianceMusic rseChangeAmbianceMusic;

    private Transform playlistParent = null;
    private Transform soundParent = null;
    private bool isPaused = false;
    private bool isFinish = false;
    private readonly Queue<AudioSource> soundsQueue = new();
    private List<AudioSource> audios = new();
    List<AudioSource> playlistAudios = new();
    int initialMusicCount;

    private void OnEnable()
    {
        rsePlayClipAt.action += PlayClipAt;
        rseOnFightStart.action += CanLaunchAudio;
        rseLoadNextLevel.action += ClearAllAudio;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
        rseChangeAmbianceMusic.action += ChangeAmbianceMusic;
    }

    private void OnDisable()
    {
        rsePlayClipAt.action -= PlayClipAt;
        rseOnFightStart.action -= CanLaunchAudio;
        rseLoadNextLevel.action -= ClearAllAudio;
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
        rseChangeAmbianceMusic.action -= ChangeAmbianceMusic;
    }

    private void Start()
    {
        SetupAudioParent();

        for (int i = 0; i < startingAudioObjectsCount; i++)
        {
            soundsQueue.Enqueue(CreateAudioSource(soundParent));
        }
    }

    private void SetupAudioParent()
    {
        playlistParent = new GameObject("PLAYLIST").transform;
        playlistParent.parent = transform;

        soundParent = new GameObject("SOUNDS").transform;
        soundParent.parent = transform;
    }

    private void CanLaunchAudio()
    {
        isFinish = false;
    }

    private void ClearAllAudio()
    {
        foreach(AudioSource audio in audios)
        {
            audio.Stop();
        }

        isFinish = true;
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void PlayClipAt(Sound sound, Vector3 position, bool isUI)
    {
        if(!isFinish || isUI)
        {
            AudioSource audioSource;

            if (soundsQueue.Count <= 0)
            {
                audioSource = CreateAudioSource(soundParent);
            }
            else
            {
                audioSource = soundsQueue.Dequeue();
            }

            audioSource.transform.position = position;
            audioSource.clip = sound.clips.GetRandom();
            audioSource.volume = Mathf.Clamp(sound.volumeMultiplier, 0, 1);
            audioSource.spatialBlend = sound.spatialBlend;

            audioSource.Play();
            StartCoroutine(AddAudioSourceToQueue(audioSource));
        }
    }

    private IEnumerator AddAudioSourceToQueue(AudioSource current)
    {
        float cooldown = current.clip.length;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;

                if (!current.isPlaying && !isFinish)
                {
                    current.UnPause();
                }
            }
            else
            {
                if (current.isPlaying)
                {
                    current.Pause();
                }
            }
        }

        soundsQueue.Enqueue(current);
    }

    private AudioSource CreateAudioSource(Transform parent)
    {
        AudioSource audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
        audioSource.transform.SetParent(parent);
        audioSource.outputAudioMixerGroup = soundMixerGroup;
        audios.Add(audioSource);
        return audioSource;
    }

    public void SetupPlaylist(Playlist[] playlists)
    {
        foreach (Playlist playlist in playlists)
        {
            AudioSource audioSource = CreatePlaylistAudioSource();
            audioSource.volume = playlist.volumMultiplier;
            audioSource.loop = playlist.isLooping;
            audioSource.clip = playlist.clip;
            audioSource.Play();
            playlistAudios.Add(audioSource);
        }
        initialMusicCount = playlists.Length;
    }
    AudioSource CreatePlaylistAudioSource()
    {
        AudioSource audioSource = new GameObject("Playlist").AddComponent<AudioSource>();
        audioSource.transform.SetParent(playlistParent);
        audioSource.outputAudioMixerGroup = musicMixerGroup;
        return audioSource;
    }

    void ChangeAmbianceMusic(Playlist[] playlists)
    {
        StartCoroutine(ChangeAmbianceMusicDelay(playlists));
    }
    IEnumerator ChangeAmbianceMusicDelay(Playlist[] playlists)
    {
        if(playlistAudios.Count > initialMusicCount)
        {
            for (int i = initialMusicCount; i < playlistAudios.Count; i++)
            {
                playlistAudios[i].DOFade(0, transitionFadeOutDelay).OnComplete(() =>
                {
                    Destroy(playlistAudios[i].gameObject);
                });
            }
        }

        yield return new WaitForSeconds(transitionFadeOutDelay);

        foreach (Playlist playlist in playlists)
        {
            AudioSource audioSource = CreatePlaylistAudioSource();
            audioSource.DOFade(playlist.volumMultiplier, transitionFadeInDelay);
            audioSource.loop = playlist.isLooping;
            audioSource.clip = playlist.clip;
            audioSource.Play();
            playlistAudios.Add(audioSource);
        }
    }
}