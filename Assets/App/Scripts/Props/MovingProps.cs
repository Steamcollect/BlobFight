using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class MovingProps : GameProps, IPausable
{
    [Header("Settings")]
    [SerializeField] float moveSpeed;
    int currentPosIndex;
    [SerializeField] float delayBeforeStart;
    [SerializeField] float delayAtPoint;
    [SerializeField] List<int> timeSpeed;
    [SerializeField] List<float> newDelayAtPoint;
    [SerializeField] List<float> newMoveSpeed;

    [Header("References")]
    [SerializeField] Transform movable;
    [SerializeField] Transform[] positions;
    [Space(20)]
    [SerializeField] WarningMovingProps warningMovingProps;

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    bool isPaused = false;

    int mode = 0;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

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
        if (positions.Length > 0)
        {
            movable.position = positions[0].position;
            StartCoroutine(DelayBeforeStart());
        }
    }
    IEnumerator DelayBeforeStart()
    {
        float cooldown = delayBeforeStart;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
                if (timer > Mathf.Max(delayBeforeStart - 1, 0))
                {
                    warningMovingProps?.onWarning.Invoke(true);
                }
            }
        }
        SetNextPos();
    }
    IEnumerator DelayAtPoint()
    {
        if(timeSpeed.Count > 0)
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

        float cooldown = delayAtPoint;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
                if(timer > Mathf.Max(delayAtPoint - 1, 0))
                {
                    warningMovingProps?.onWarning.Invoke(true);
                }
            }
        }
        SetNextPos();
    }

    void SetNextPos()
    {
        currentPosIndex = (currentPosIndex + 1) % positions.Length;
        float moveTime = Vector2.Distance(movable.position, positions[currentPosIndex].position) / moveSpeed;
        movable.DOMove(positions[currentPosIndex].position, moveTime).OnComplete(() =>
        {
            StartCoroutine(DelayAtPoint());
        });
    }

    private void OnDestroy()
    {
        movable.DOKill();
    }

    public void Pause()
    {
        isPaused = true;
        movable.DOPause();
    }

    public void Resume()
    {
        isPaused = false;
        movable.DOPlay();
    }
}