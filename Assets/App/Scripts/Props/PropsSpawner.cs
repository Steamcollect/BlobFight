using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawner : GameProps
{
    [Header("Settings")]
    [SerializeField] Vector2 spawnCooldown;
    [SerializeField] float spawnForce;
    [SerializeField] List<int> timeSpeed;
    [SerializeField] List<Vector2> newtimeSpeed;

    [Space(5)]
    [SerializeField] int objToSpawnOnStart;

    [Header("References")]
    [SerializeField] Transform spawner;
    [SerializeField] Rigidbody2D objToSpawn;

    Queue<Rigidbody2D> objQueue = new();

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    int mode = 0;
    bool isPaused = false;

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

    public override void Launch()
    {
        StartCoroutine(SpawnCooldown());
    }

    private void Awake()
    {
        for (int i = 0; i < objToSpawnOnStart; i++)
        {
            CreateObj();
        }
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    IEnumerator SpawnCooldown()
    {
        if (timeSpeed.Count > 0)
        {
            if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
            {
                spawnCooldown = newtimeSpeed[mode];

                if (mode < timeSpeed.Count - 1)
                {
                    mode++;
                }
            }
        }

        float cooldown = Random.Range(spawnCooldown.x, spawnCooldown.y);
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if(!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

        Rigidbody2D obj = GetObj();
        obj.gameObject.SetActive(true);
        obj.transform.position = spawner.position;
        obj.AddForce(spawner.up * spawnForce);

        StartCoroutine(SpawnCooldown());
    }

    Rigidbody2D GetObj()
    {
        if(objQueue.Count <= 0) CreateObj();

        Rigidbody2D obj = objQueue.Dequeue();
        objQueue.Enqueue(obj);
        return obj;
    }
    void CreateObj()
    {
        Rigidbody2D obj = Instantiate(objToSpawn);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(spawner);
        obj.gameObject.AddComponent<CustomCollision>();

        objQueue.Enqueue(obj);
    }
}