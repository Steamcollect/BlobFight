using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderSpawner : GameProps
{
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private float delayAfterGameStart;
    [SerializeField] private Vector2 delayBetweenLightning;
    [SerializeField] private int objToSpawnOnStart;
    [SerializeField] private List<int> timeSpeed;
    [SerializeField] private List<Vector2> newDelayBetweenLightning;

    [Header("References")]
    [SerializeField] private ThunderProps thunderPrefab;
    [SerializeField] private List<Transform> spawnPoint;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

    private bool isPaused = false;
    private int mode = 0;
    private Queue<ThunderProps> thunderQueue = new();
    private List<int> availableSpawns = new();
    
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

    public override void Launch()
    {
        for (int i = 0; i < spawnPoint.Count; i++)
        {
            availableSpawns.Add(i);
        }

        StartCoroutine(StartDelaySpawn());
    }

    private IEnumerator StartDelaySpawn()
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

        StartCoroutine(DelaySpawnThunder());
    }

    private IEnumerator DelaySpawnThunder()
    {
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

        SpawnThunder();
    }

    private void SpawnThunder()
    {
        if (availableSpawns.Count > 0)
        {
            int rndIndex = Random.Range(0, availableSpawns.Count);
            int spawnIdx = availableSpawns[rndIndex];
            availableSpawns.RemoveAt(rndIndex);

            ThunderProps thunder = GetThunderObj();
            thunder.SetRandomSpawn(spawnIdx);
            thunder.gameObject.SetActive(true);
            thunder.PlaySound();
            thunder.transform.position = spawnPoint[spawnIdx].position;
            thunder.Flip(Random.value < 0.5f);
            thunder.onEndAction += ResetSpawnPoint;
        }

        if (timeSpeed.Count > 0)
        {
            if (mode < timeSpeed.Count && rsoTimerParty.Value >= timeSpeed[mode])
            {
                delayBetweenLightning = newDelayBetweenLightning[mode];

                if (mode < timeSpeed.Count - 1)
                {
                    mode++;
                }
            }
        }

        StartCoroutine(DelaySpawnThunder());
    }

    private ThunderProps GetThunderObj()
    {
        if (thunderQueue.Count == 0)
        {
            CreateThunderObj();
        }

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

    private void ResetSpawnPoint(ThunderProps thunder)
    {
        thunder.onEndAction -= ResetSpawnPoint;
        availableSpawns.Add(thunder.GetRandomSpawn());
    }
}