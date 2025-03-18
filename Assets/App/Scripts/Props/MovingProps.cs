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

    int mode = 0;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

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
        yield return new WaitForSeconds(delayBeforeStart);
        
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


        yield return new WaitForSeconds(delayAtPoint);
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
        movable.DOPause();
    }

    public void Resume()
    {
        movable.DOPlay();
    }
}