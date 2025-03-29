using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Output")]
    [SerializeField] private RSO_TimerGame rsoTimerGame;
    [SerializeField] private RSO_TimerParty rsoTimerParty;

    [Header("Input")]
    [SerializeField] private RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    bool isPaused = false;
    private int totalSecondesGame = 0;
    private int totalSecondesParty = 0;
    private Coroutine timerPartyCoroutine = null;

    private void OnEnable()
    {
        rseOnFightStart.action += StartTimerParty;
        rseOnFightEnd.action += StopTimerParty;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseOnFightStart.action -= StartTimerParty;
        rseOnFightEnd.action -= StopTimerParty;
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void Start()
    {
        rsoTimerGame.Value = totalSecondesGame;
        rsoTimerParty.Value = totalSecondesParty;

        InvokeRepeating(nameof(IncrementGameTimer), 1f, 1f);
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void IncrementGameTimer()
    {
        totalSecondesGame += 1;
        rsoTimerGame.Value = totalSecondesGame;
    }

    private void StartTimerParty()
    {
        totalSecondesParty = 0;
        rsoTimerParty.Value = totalSecondesParty;

        if (timerPartyCoroutine != null)
        {
            StopCoroutine(timerPartyCoroutine);
            timerPartyCoroutine = null;
        }

        timerPartyCoroutine = StartCoroutine(IncrementTimerParty());
    }

    private void StopTimerParty()
    {
        totalSecondesParty = 0;

        if (timerPartyCoroutine != null)
        {
            StopCoroutine(timerPartyCoroutine);
            timerPartyCoroutine = null;
        }
    }

    private IEnumerator IncrementTimerParty()
    {
        float cooldown = 1;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

        totalSecondesParty += 1;
        rsoTimerParty.Value = totalSecondesParty;

        timerPartyCoroutine = StartCoroutine(IncrementTimerParty());
    }
}
