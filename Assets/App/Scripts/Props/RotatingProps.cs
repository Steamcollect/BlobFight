using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingProps : GameProps
{
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float delayBeforeStart;
    [SerializeField] private float delayTransit;
    [SerializeField] private List<int> timeSpeed;
    [SerializeField] private List<float> newtimeSpeed;

    [Header("Output")]
    [SerializeField] private RSO_TimerParty rsoTimerParty;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private bool isLaunched = false;
    private int mode = 0;
    private bool isPaused = false;
    private Coroutine speedTransitionCoroutine;

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
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        isLaunched = true;
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (!isLaunched) return;

            Rotate();
        }
    }

    private void Rotate()
    {
        if (timeSpeed.Count > 0)
        {
            if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
            {
                if (speedTransitionCoroutine != null)
                {
                    StopCoroutine(speedTransitionCoroutine);
                }     

                speedTransitionCoroutine = StartCoroutine(SmoothSpeedTransition(rotationSpeed, newtimeSpeed[mode], delayTransit));

                if (mode < timeSpeed.Count - 1)
                {
                    mode++;
                }
            }
        }

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator SmoothSpeedTransition(float startSpeed, float targetSpeed, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rotationSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rotationSpeed = targetSpeed;
    }
}