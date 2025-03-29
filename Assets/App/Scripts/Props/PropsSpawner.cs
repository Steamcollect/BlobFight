using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawner : GameProps
{
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private Vector2 spawnCooldown;
    [SerializeField] private float spawnForce;
    [SerializeField] private int objToSpawnOnStart;
    [SerializeField] private List<int> timeSpeed;
    [SerializeField] private List<Vector2> newtimeSpeed;

    [Header("References")]
    [SerializeField] private Transform spawner;
    [SerializeField] private Rigidbody2D objToSpawn;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    [Header("Output")]
    [SerializeField] private RSO_TimerParty rsoTimerParty;

    private Queue<Rigidbody2D> objQueue = new();
    private int mode = 0;
    private bool isPaused = false;

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

    private IEnumerator SpawnCooldown()
    {
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

        SpawnProps();
    }

    private void SpawnProps()
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

        Rigidbody2D obj = GetObj();
        obj.gameObject.SetActive(true);
        obj.transform.position = spawner.position;
        obj.AddForce(spawner.up * spawnForce);

        StartCoroutine(SpawnCooldown());
    }

    private Rigidbody2D GetObj()
    {
        if (objQueue.Count <= 0) CreateObj();

        Rigidbody2D obj = objQueue.Dequeue();
        objQueue.Enqueue(obj);
        return obj;
    }

    private void CreateObj()
    {
        Rigidbody2D obj = Instantiate(objToSpawn);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(spawner);
        obj.gameObject.AddComponent<CustomCollision>();

        objQueue.Enqueue(obj);
    }
}