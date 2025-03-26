using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ThunderSpawner : GameProps
{
    [Header("Settings")]
    [SerializeField] float delayAfterGameStart;
    //[SerializeField] AnimationCurve curveDifficulty;
    [SerializeField] Vector2 delayBetweenLightning;
    [Space(5)]
    [SerializeField] int objToSpawnOnStart;
    [SerializeField] List<int> timeSpeed;
    [SerializeField] List<Vector2> newDelayBetweenLightning;
    private List<int> canSpawn = new();
    [Header("References")]
    [SerializeField] List<Transform> spawnPoint;
    [SerializeField] ThunderProps thunderPrefab;

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

    Queue<ThunderProps> thunderQueue = new();
    int mode = 0;

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    bool isPaused = false;
    
    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void Awake()
    {
        for (int i = 0; i < objToSpawnOnStart; i++)
        {
            CreateThunderObj();
        }
    }

    private new void OnEnable()
    {
        base.OnEnable();
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }
    private new void OnDisable()
    {
        base.OnDisable();
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }
    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void StartCoroutineDelay()
    {
        StartCoroutine(StartDelaySpawn());
    }
    IEnumerator StartDelaySpawn()
    {
        float cooldown = delayAfterGameStart;
        float timer = 0f;

        while (timer < delayAfterGameStart)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

        StartCoroutine(SpawnThunder());
    }
    IEnumerator SpawnThunder()
    {
        if (timeSpeed.Count > 0)
        {
            if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
            {
                delayBetweenLightning = newDelayBetweenLightning[mode];

                if (mode < timeSpeed.Count - 1)
                {
                    mode++;
                }
            }
        }

        float cooldown = Random.Range(delayBetweenLightning.x, delayBetweenLightning.y);
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

        if(canSpawn.Count > 0)
        {
            int rnd = Random.Range(0, canSpawn.Count);

            ThunderProps thunder = GetThunderObj();
            thunder.randomSpawn = canSpawn[rnd];
            thunder.gameObject.SetActive(true);
            thunder.PlaySound();
            thunder.transform.position = spawnPoint[canSpawn[rnd]].position;
            thunder.Flip(Random.value < 0.5f);
            thunder.onEndAction += ResetSpawnPoint;

            canSpawn.RemoveAt(rnd);
        }
        StartCoroutine(SpawnThunder());
    }
    ThunderProps GetThunderObj()
    {
        if (thunderQueue.Count <= 0) CreateThunderObj();

        return thunderQueue.Dequeue();
    }
    private void CreateThunderObj()
    {
        ThunderProps obj = Instantiate(thunderPrefab);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(spawnPoint[0]);
        obj.onEndAction += QueueThunder;
        thunderQueue.Enqueue(obj);
    }
    private void QueueThunder(ThunderProps thunder)
    {
        thunderQueue.Enqueue(thunder);
    }

    public override void Launch()
    {
        for (int i = 0; i < spawnPoint.Count; i++)
        {
            canSpawn.Add(i);
        }
        StartCoroutineDelay();
    }
    private void ResetSpawnPoint(ThunderProps thunder)
    {
        thunder.onEndAction -= ResetSpawnPoint;
        canSpawn.Add(thunder.randomSpawn);
    }
}