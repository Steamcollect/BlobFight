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

    private int totalSecondesGame;
    private int totalSecondesParty;

    private Coroutine timerParty;

    private void OnEnable()
    {
        rseOnFightStart.action += StartTimerParty;
        rseOnFightEnd.action += StopTimerParty;
    }

    private void OnDisable()
    {
        rseOnFightStart.action -= StartTimerParty;
        rseOnFightEnd.action -= StopTimerParty;
    }

    void Start()
    {
        totalSecondesGame = 0;
        totalSecondesParty = 0;

        rsoTimerGame.Value = totalSecondesGame;
        rsoTimerParty.Value = totalSecondesParty;

        StartCoroutine(TimerGame());
    }

    IEnumerator TimerGame()
    {
        yield return new WaitForSeconds(1);

        totalSecondesGame++;
        rsoTimerGame.Value = totalSecondesGame;

        StartCoroutine(TimerGame());
    }

    private void StartTimerParty()
    {
        totalSecondesParty = 0;
        rsoTimerParty.Value = totalSecondesParty;

        if (timerParty != null)
        {
            StopCoroutine(timerParty);
            timerParty = null;
        }

        timerParty = StartCoroutine(TimerParty());
    }

    private void StopTimerParty()
    {
        totalSecondesParty = 0;

        if (timerParty != null)
        {
            StopCoroutine(timerParty);
            timerParty = null;
        }
    }

    IEnumerator TimerParty()
    {
        yield return new WaitForSeconds(1);

        totalSecondesParty++;
        rsoTimerParty.Value = totalSecondesParty;

        timerParty = StartCoroutine(TimerParty());
    }
}
