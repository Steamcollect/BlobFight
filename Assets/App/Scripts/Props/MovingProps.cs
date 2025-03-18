using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class MovingProps : GameProps, IPausable
{
    [Header("Settings")]
    [SerializeField] bool haveWarning;
    [SerializeField] float moveSpeed;
    int currentPosIndex;
    bool isVisible = false;
    [SerializeField] float delayBeforeStart;
    [SerializeField] float delayAtPoint;
    [SerializeField] List<int> timeSpeed;
    [SerializeField] List<float> newDelayAtPoint;
    [SerializeField] List<float> newMoveSpeed;

    [Header("References")]
    [SerializeField] Transform movable;
    [SerializeField] Transform[] positions;
    [SerializeField] RSE_UpdateWarning RSE_UpdateWarning;

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
        RSE_UpdateWarning.Call(!Checkisibility());

        if (positions.Length > 0)
        {
            movable.position = positions[0].position;
            StartCoroutine(DelayBeforeStart());
        }
    }
    private void FixedUpdate()
    {
        bool visible = Checkisibility();
        if (haveWarning && visible != isVisible)
        {
            isVisible = visible;
            RSE_UpdateWarning.Call(!visible);
        }
    }
    bool Checkisibility()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        return GeometryUtility.TestPlanesAABB(planes, new Bounds(movable.position, Vector3.one));
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