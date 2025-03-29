using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class MovingProps : GameProps
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isLava;
    [SerializeField] private float delayBeforeStart;
    [SerializeField] private float delayAtPoint;
    [SerializeField] private List<int> timeSpeed;
    [SerializeField] private List<float> newDelayAtPoint;
    [SerializeField] private List<float> newMoveSpeed;

    [Header("References")]
    [SerializeField] private Transform movable;
    [SerializeField] private WarningMovingProps warningMovingProps;
    [SerializeField] private Transform[] positions;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

    private bool isPaused = false;
    private int currentPosIndex = 0;
    private int mode = 0;

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

    private void OnDestroy()
    {
        movable.DOKill();
    }

    private void Pause()
    {
        isPaused = true;
        movable.DOPause();
    }

    private void Resume()
    {
        isPaused = false;
        movable.DOPlay();
    }

    public override void Launch()
    {
        if (positions.Length > 0)
        {
            movable.position = positions[0].position;
            StartCoroutine(DelayLaunch(delayBeforeStart));
        }
    }

    private IEnumerator DelayLaunch(float delay)
    {
        float cooldown = delay;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
                if (timer > Mathf.Max(delay - 1, 0))
                {
                    warningMovingProps?.onWarning.Invoke(true);
                }
            }
        }

        SetNextPos();
    }

    private void SetNextPos()
    {
        if (!isLava)
        {
            movable.gameObject.SetActive(true);
        }
		
		currentPosIndex = (currentPosIndex + 1) % positions.Length;
        float moveTime = Vector2.Distance(movable.position, positions[currentPosIndex].position) / moveSpeed;

        if (timeSpeed.Count > 0)
        {
            if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
            {
                delayAtPoint = newDelayAtPoint[mode];
                moveSpeed = newMoveSpeed[mode];

                if (mode < timeSpeed.Count - 1)
                {
                    mode++;
                }
            }
        }

        movable.DOMove(positions[currentPosIndex].position, moveTime).OnComplete(() =>
        {
            if (!isLava)
            {
                movable.gameObject.SetActive(false);
            }

            StartCoroutine(DelayLaunch(delayAtPoint));
		});
    }
}